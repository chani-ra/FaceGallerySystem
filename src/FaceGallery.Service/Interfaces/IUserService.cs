// הנתיב המלא: C#-2026-service/Interfaces/IUserService.cs
using c__service_2026.Dto;
using System.Threading.Tasks;

namespace c__service_2026.Interfaces
{
    public interface IUserService : IService<UserDto>
    {
        Task<UserDto> RegisterAsync(UserDto userDto);
        Task<UserDto> LoginAsync(string emailOrUsername, string password);
    }
}
/*
 * מטרת העמוד:
 * ממשק ייעודי לניהול המשתמשים באתר.
 * בנוסף לפעולות הרגילות (שמגיעות מהירושה של IService), הוא מגדיר
 * את שתי הפעולות הקריטיות ביותר לאתר אמיתי: הרשמה (Register) והתחברות (Login).
 */