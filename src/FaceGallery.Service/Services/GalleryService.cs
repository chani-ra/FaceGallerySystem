// הנתיב המלא: C#-2026-service/Services/GalleryService.cs
using AutoMapper;
using c__nRepository_2026.Entities;
using c__nRepository_2026.Interfaces;
using c__service_2026.Dto;
using c__service_2026.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace c__service_2026.Services
{
    public class GalleryService : IGalleryService
    {
        private readonly IGalleryRepository _galleryRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IDetectedRepository _detectionRepository;
        private readonly IMapper _mapper; // הזרקת ה-AutoMapper!

        public GalleryService(
            IGalleryRepository galleryRepository,
            IImageRepository imageRepository,
            IDetectedRepository detectionRepository,
            IMapper mapper)
        {
            _galleryRepository = galleryRepository;
            _imageRepository = imageRepository;
            _detectionRepository = detectionRepository;
            _mapper = mapper;
        }

        public async Task<List<GalleryDto>> GetAllAsync()
        {
            var galleries = await _galleryRepository.GetAllAsync();
            return await MapToDTOListAsync(galleries);
        }

        public async Task<GalleryDto> GetByIdAsync(int id)
        {
            var gallery = await _galleryRepository.GetByIdAsync(id);
            return gallery == null ? null : await MapToDTOAsync(gallery);
        }

        public async Task<GalleryDto> AddItemAsync(GalleryDto item)
        {
            if (string.IsNullOrWhiteSpace(item.Name)) throw new ArgumentException("שם הגלריה הוא שדה חובה");

            // המרה מהלקוח למסד הנתונים
            var gallery = _mapper.Map<Gallery>(item);
            gallery.CreatedDate = DateTime.Now;

            var result = await _galleryRepository.AddItemAsync(gallery);
            return await MapToDTOAsync(result);
        }

        public async Task UpdateItemAsync(int id, GalleryDto item)
        {
            if (string.IsNullOrWhiteSpace(item.Name)) throw new ArgumentException("שם הגלריה הוא שדה חובה");

            var existingGallery = await _galleryRepository.GetByIdAsync(id);
            if (existingGallery == null) throw new ArgumentException("הגלריה לא נמצאה");

            // עדכון השדות
            existingGallery.Name = item.Name;
            existingGallery.CharacterId = item.CharacterId;

            await _galleryRepository.UpdateItemAsync(id, existingGallery);
        }

        public async Task DeleteItemAsync(int id)
        {
            await _galleryRepository.DeleteItemAsync(id);
        }

        // --- לוגיקה ייחודית ---

        public async Task<List<GalleryDto>> GetByUserAsync(int userId)
        {
            var galleries = await _galleryRepository.GetGalleriesByUserIdAsync(userId);
            return await MapToDTOListAsync(galleries);
        }

        public async Task<List<GalleryDto>> GetByCharacterAsync(int characterId)
        {
            var galleries = await _galleryRepository.GetGalleriesByCharacterIdAsync(characterId);
            return await MapToDTOListAsync(galleries);
        }

        public async Task<GalleryDto> GetWithImagesAsync(int galleryId)
        {
            return await GetByIdAsync(galleryId); // בפונקציית המיפוי שלנו אנחנו כבר דואגים להביא את התמונות!
        }

        public async Task<Dictionary<string, object>> GetGalleryStatisticsAsync(int galleryId)
        {
            var gallery = await _galleryRepository.GetByIdAsync(galleryId);
            if (gallery == null) throw new ArgumentException("הגלריה לא נמצאה");

            var images = await _imageRepository.GetImagesByGalleryIdAsync(galleryId);
            int totalDetections = 0;
            double sumConfidence = 0;

            // עוברים על כל התמונות בגלריה כדי לאסוף סטטיסטיקות על הזיהויים שבהן
            foreach (var image in images)
            {
                var detections = await _detectionRepository.GetDetectionsByImageIdAsync(image.Id);
                totalDetections += detections.Count;
                if (detections.Count > 0) sumConfidence += detections.Sum(d => d.Confidence);
            }

            return new Dictionary<string, object>
            {
                { "GalleryId", galleryId },
                { "GalleryName", gallery.Name },
                { "TotalImages", images.Count },
                { "TotalDetections", totalDetections },
                { "AverageConfidence", totalDetections > 0 ? Math.Round((sumConfidence / totalDetections) * 100, 2) : 0 }
            };
        }

        // --- פונקציות עזר פרטיות למיפוי ---

        private async Task<GalleryDto> MapToDTOAsync(Gallery gallery)
        {
            // המרה בסיסית דרך AutoMapper
            var dto = _mapper.Map<GalleryDto>(gallery);

            // שליפת התמונות השייכות לגלריה הזו והוספתן ל-DTO
            var images = await _imageRepository.GetImagesByGalleryIdAsync(gallery.Id);
            dto.ImageCount = images.Count;
            dto.Images = _mapper.Map<List<ImageDto>>(images);

            // אם נרצה להציג את שם הדמות, נוכל למשוך אותו גם (בהנחה שהוא נטען מה-DB)
            if (gallery.Character != null)
            {
                dto.CharacterName = gallery.Character.CharacterName;
            }

            return dto;
        }

        private async Task<List<GalleryDto>> MapToDTOListAsync(IEnumerable<Gallery> galleries)
        {
            var list = new List<GalleryDto>();
            foreach (var g in galleries)
            {
                list.Add(await MapToDTOAsync(g));
            }
            return list;
        }
    }
}
