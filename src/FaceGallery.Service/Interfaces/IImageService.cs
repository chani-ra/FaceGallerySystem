// הנתיב המלא: C#-2026-service/Interfaces/IImageService.cs
using c__service_2026.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace c__service_2026.Interfaces
{
    public interface IImageService : IService<ImageDto>
    {
        Task<List<ImageDto>> GetByGalleryAsync(int galleryId);
        Task<List<ImageDto>> GetByUserAsync(int userId);
        Task<List<ImageDto>> GetUnprocessedAsync();
        Task<bool> MarkAsProcessedAsync(int imageId);
        Task<ImageDto> GetWithDetectionsAsync(int imageId);

        // פונקציית הקסם שמתקשרת עם Azure
        Task<ImageDto> ProcessImageWithDetectionAsync(ImageDto item);
    }
}
/*
 * מטרת העמוד:
 * הממשק המרכזי ביותר באתר, שאחראי על התמונות עצמן.
 * הוא מכיל את ההגדרה של פונקציית העיבוד (ProcessImageWithDetectionAsync),
 * שתיקח תמונה חדשה ותשלח אותה לסריקה מלאה מול שרתי ה-AI.
 */
