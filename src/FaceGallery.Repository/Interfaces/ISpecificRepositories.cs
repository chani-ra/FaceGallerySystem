// הנתיב המלא: C#-nRepository-2026/Interfaces/ISpecificRepositories.cs
using c__nRepository_2026.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace c__nRepository_2026.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        // פונקציות ייחודיות למשתמשים - חובה בשביל תהליך התחברות (Login)
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
    }
    public interface ICharacterRepository : IRepository<Character>
    {
        Task<List<Character>> SearchByNameAsync(string name);
    }

    public interface IDetectedRepository : IRepository<DetectedCharacter>
    {
        Task<List<DetectedCharacter>> GetDetectionsByImageIdAsync(int imageId);
        Task<List<DetectedCharacter>> GetDetectionsByCharacterIdAsync(int characterId);
    }

    public interface IGalleryRepository : IRepository<Gallery>
    {
        Task<List<Gallery>> GetGalleriesByUserIdAsync(int userId);
        Task<List<Gallery>> GetGalleriesByCharacterIdAsync(int characterId);
    }

    public interface IImageRepository : IRepository<Image>
    {
        Task<List<Image>> GetImagesByGalleryIdAsync(int galleryId);
        Task<List<Image>> GetImagesByUserIdAsync(int userId);
    }
}
/*
 * מטרת הקובץ (Specific Interfaces):
 * יצירת ממשקים ייעודיים לכל טבלה שיורשים מהממשק הגנרי IRepository.
 * זה קריטי כדי שנוכל להגדיר פונקציות חיפוש ושליפה ייחודיות (כמו שליפת תמונות לפי גלריה)
 * וששכבת ה-Service תוכל להכיר אותן דרך הזרקת התלויות (DI).
 */