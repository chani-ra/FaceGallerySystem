// הנתיב המלא: C#-2026-service/Interfaces/ICharacterService.cs
using c__service_2026.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace c__service_2026.Interfaces
{
    public interface ICharacterService : IService<CharacterDto>
    {
        Task<List<CharacterDto>> SearchByNameAsync(string name);
        Task<List<CharacterDto>> GetTopDetectedAsync(int topCount);
        Task<Dictionary<string, object>> GetCharacterStatisticsAsync(int characterId);
    }
}
/*
 * מטרת העמוד:
 * ממשק ייעודי ללוגיקה של ניהול דמויות במערכת.
 * מגדיר פעולות מתקדמות שחשובות לחוויית המשתמש, כמו חיפוש חכם של דמות לפי שם,
 * שליפת הדמויות הפופולריות ביותר (למשל עבור דף הבית), והפקת סטטיסטיקות על דמות.
 */