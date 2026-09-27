// הנתיב המלא: C#-2026-service/Interfaces/IGalleryService.cs
using c__service_2026.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace c__service_2026.Interfaces
{
    public interface IGalleryService : IService<GalleryDto>
    {
        Task<List<GalleryDto>> GetByUserAsync(int userId);
        Task<List<GalleryDto>> GetByCharacterAsync(int characterId);
        Task<GalleryDto> GetWithImagesAsync(int galleryId);
        Task<Dictionary<string, object>> GetGalleryStatisticsAsync(int galleryId);
    }
}
/*
 * מטרת העמוד:
 * ממשק לניהול הלוגיקה של הגלריות השונות.
 * מכיל חתימות לשליפת הגלריות האישיות של משתמש ספציפי (קריטי לאתר!),
 * וכן שליפת גלריה יחד עם כל התמונות שבתוכה בבקשה אחת.
 */