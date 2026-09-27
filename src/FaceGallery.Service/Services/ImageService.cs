// הנתיב המלא: C#-2026-service/Services/ImageService.cs
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
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IDetectedRepository _detectionRepository;
        private readonly IAzureVisionService _azureVisionService;
        private readonly IMapper _mapper;

        public ImageService(
            IImageRepository imageRepository,
            IDetectedRepository detectionRepository,
            IAzureVisionService azureVisionService,
            IMapper mapper)
        {
            _imageRepository = imageRepository;
            _detectionRepository = detectionRepository;
            _azureVisionService = azureVisionService;
            _mapper = mapper;
        }

        public async Task<List<ImageDto>> GetAllAsync()
        {
            var images = await _imageRepository.GetAllAsync();
            return await MapToDTOListAsync(images);
        }

        public async Task<ImageDto> GetByIdAsync(int id)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            return image == null ? null : await MapToDTOAsync(image);
        }

        public async Task<ImageDto> AddItemAsync(ImageDto item)
        {
            if (string.IsNullOrEmpty(item.Url)) throw new ArgumentException("URL תמונה הוא שדה חובה");

            var imageEntity = _mapper.Map<Image>(item);
            var result = await _imageRepository.AddItemAsync(imageEntity);
            return await MapToDTOAsync(result);
        }

        public async Task UpdateItemAsync(int id, ImageDto item)
        {
            var existing = await _imageRepository.GetByIdAsync(id);
            if (existing == null) throw new Exception("תמונה לא נמצאה");

            existing.Url = item.Url;
            existing.GalleryId = item.GalleryId;
            await _imageRepository.UpdateItemAsync(id, existing);
        }

        public async Task DeleteItemAsync(int id)
        {
            await _imageRepository.DeleteItemAsync(id);
        }

        // --- לוגיקה ייחודית ועיבוד AI ---

        public async Task<List<ImageDto>> GetByGalleryAsync(int galleryId)
        {
            var images = await _imageRepository.GetImagesByGalleryIdAsync(galleryId);
            return await MapToDTOListAsync(images);
        }

        public async Task<List<ImageDto>> GetByUserAsync(int userId)
        {
            var images = await _imageRepository.GetImagesByUserIdAsync(userId);
            return await MapToDTOListAsync(images);
        }

        public async Task<List<ImageDto>> GetUnprocessedAsync()
        {
            var allImages = await _imageRepository.GetAllAsync();
            var unprocessed = new List<ImageDto>();
            foreach (var i in allImages)
            {
                var detections = await _detectionRepository.GetDetectionsByImageIdAsync(i.Id);
                if (!detections.Any()) unprocessed.Add(await MapToDTOAsync(i));
            }
            return unprocessed;
        }

        public async Task<bool> MarkAsProcessedAsync(int imageId)
        {
            return await Task.FromResult(true); // מחושב אוטומטית לפי קיום זיהויים
        }

        public async Task<ImageDto> GetWithDetectionsAsync(int imageId)
        {
            return await GetByIdAsync(imageId); // המיפוי מביא את הזיהויים אוטומטית
        }

        // פונקציית הקסם - החיבור ל-Azure!
        public async Task<ImageDto> ProcessImageWithDetectionAsync(ImageDto item)
        {
            if (string.IsNullOrEmpty(item.Url)) throw new ArgumentException("URL תמונה חובה");

            // 1. קודם שומרים את התמונה החדשה ב-DB
            var imageEntity = new Image { Url = item.Url, GalleryId = item.GalleryId, UserId = item.UserId };
            var savedImage = await _imageRepository.AddItemAsync(imageEntity);

            // 2. שולחים את ה-URL למוח של מיקרוסופט (Azure)
            var faces = await _azureVisionService.DetectFacesAsync(item.Url);

            // 3. אם Azure מצא פנים, אנחנו שומרים את המיקומים שלהם בטבלת הזיהויים!
            if (faces.Any())
            {
                foreach (var face in faces)
                {
                    await _detectionRepository.AddItemAsync(new DetectedCharacter
                    {
                        ImageId = savedImage.Id,
                        CharacterId = 1, // הערה: מזהה דמות ברירת מחדל עד לסיווג ידני
                        Confidence = (float)face.Confidence,
                        FaceCoordinates = face.FaceCoordinates,
                        DetectionDate = DateTime.Now
                    });
                }
            }

            // 4. מחזירים את התמונה המלאה (עם כל הפרצופים שזוהו עליה) ללקוח
            return await MapToDTOAsync(savedImage);
        }

        // --- פונקציות עזר פרטיות למיפוי ---

        private async Task<ImageDto> MapToDTOAsync(Image image)
        {
            var dto = _mapper.Map<ImageDto>(image);

            // שולפים את כל הפרצופים שזוהו בתמונה הזו ומצרפים אותם לאובייקט
            var detections = await _detectionRepository.GetDetectionsByImageIdAsync(image.Id);
            dto.DetectionCount = detections.Count;
            dto.IsProcessed = detections.Count > 0; // לוגיקה חכמה: עובד אם יש זיהויים
            dto.Detections = _mapper.Map<List<DetectedCharacterDto>>(detections);

            return dto;
        }

        private async Task<List<ImageDto>> MapToDTOListAsync(IEnumerable<Image> images)
        {
            var list = new List<ImageDto>();
            foreach (var i in images) list.Add(await MapToDTOAsync(i));
            return list;
        }
    }
}

