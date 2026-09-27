// הנתיב המלא: C#-2026-service/Interfaces/IDetectedService.cs
using c__service_2026.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace c__service_2026.Interfaces
{
    public interface IDetectedService : IService<DetectedCharacterDto>
    {
        Task<List<DetectedCharacterDto>> GetByImageAsync(int imageId);
        Task<List<DetectedCharacterDto>> GetByCharacterAsync(int characterId);
        Task<List<DetectedCharacterDto>> GetHighConfidenceAsync(double minConfidence);
        Task<Dictionary<string, object>> GetDetectionStatisticsAsync();
    }
}
/*
 * מטרת העמוד:
 * הגדרת הפעולות עבור תוצאות הזיהוי שחוזרות מאלגוריתם ה-AI.
 * מאפשר שליפת זיהויים ברמת ביטחון גבוהה (Confidence), ושליפת כל
 * הזיהויים ששייכים לתמונה ספציפית או לדמות ספציפית.
 */