using c__service_2026.Dto;
using c__service_2026.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FaceGallery.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GalleriesController : ControllerBase
    {
        private readonly IGalleryService _galleryService;

        public GalleriesController(IGalleryService galleryService)
        {
            _galleryService = galleryService;
        }

        // GET api/galleries
        [HttpGet]
        public async Task<ActionResult<List<GalleryDto>>> GetAll()
        {
            return Ok(await _galleryService.GetAllAsync());
        }

        // GET api/galleries/5  (includes its images)
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GalleryDto>> GetById(int id)
        {
            var gallery = await _galleryService.GetWithImagesAsync(id);
            if (gallery == null) return NotFound();
            return Ok(gallery);
        }

        public class CreateGalleryRequest
        {
            public string Name { get; set; } = string.Empty;
        }

        // POST api/galleries
        // Creates a gallery under the seeded demo user/character (this is a
        // single-user demo - see docs/ARCHITECTURE.md for how to extend to
        // real accounts using the existing User entity/IUserService).
        [HttpPost]
        public async Task<ActionResult<GalleryDto>> Create([FromBody] CreateGalleryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Name is required.");

            var created = await _galleryService.AddItemAsync(new GalleryDto
            {
                Name = request.Name,
                CharacterId = 1,
                UserId = 1
            });

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // DELETE api/galleries/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _galleryService.DeleteItemAsync(id);
            return NoContent();
        }
    }
}
