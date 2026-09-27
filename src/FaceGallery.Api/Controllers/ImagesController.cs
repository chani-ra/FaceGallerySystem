using c__service_2026.Dto;
using c__service_2026.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FaceGallery.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class ImagesController : ControllerBase
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".bmp" };
        private const long MaxFileSizeBytes = 15 * 1024 * 1024; // 15 MB

        private readonly IImageService _imageService;
        private readonly IWebHostEnvironment _env;

        public ImagesController(IImageService imageService, IWebHostEnvironment env)
        {
            _imageService = imageService;
            _env = env;
        }

        // GET api/galleries/5/images
        [HttpGet("galleries/{galleryId:int}/images")]
        public async Task<ActionResult<List<ImageDto>>> GetByGallery(int galleryId)
        {
            return Ok(await _imageService.GetByGalleryAsync(galleryId));
        }

        // POST api/galleries/5/images  (multipart/form-data, field name "file")
        // Adds one photo to the gallery's existing collection. This does NOT
        // call Azure - it just stores the photo so it can be searched later
        // by /api/recognize.
        [HttpPost("galleries/{galleryId:int}/images")]
        [RequestSizeLimit(MaxFileSizeBytes)]
        public async Task<ActionResult<ImageDto>> Upload(int galleryId, IFormFile file)
        {
            var validation = ValidateFile(file);
            if (validation != null) return BadRequest(validation);

            var relativeUrl = await SaveFileAsync(file, galleryId);

            var saved = await _imageService.AddItemAsync(new ImageDto
            {
                Url = relativeUrl,
                GalleryId = galleryId,
                UserId = 1
            });

            return CreatedAtAction(nameof(GetByGallery), new { galleryId }, saved);
        }

        // DELETE api/images/5
        [HttpDelete("images/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _imageService.DeleteItemAsync(id);
            return NoContent();
        }

        private string? ValidateFile(IFormFile? file)
        {
            if (file == null || file.Length == 0) return "No file was uploaded.";
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext)) return $"Unsupported file type '{ext}'.";
            return null;
        }

        private async Task<string> SaveFileAsync(IFormFile file, int galleryId)
        {
            var webRoot = _env.WebRootPath;
            var folder = Path.Combine(webRoot, "uploads", galleryId.ToString());
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName).ToLowerInvariant()}";
            var fullPath = Path.Combine(folder, fileName);

            await using (var stream = System.IO.File.Create(fullPath))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{galleryId}/{fileName}";
        }
    }
}
