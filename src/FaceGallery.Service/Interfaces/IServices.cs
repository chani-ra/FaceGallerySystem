// הנתיב המלא: C#-2026-service/Interfaces/IService.cs
using System.Collections.Generic;
using System.Threading.Tasks;

namespace c__service_2026.Interfaces
{
    public interface IService<T>
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> AddItemAsync(T item);
        Task UpdateItemAsync(int id, T item);
        Task DeleteItemAsync(int id);
    }
}
/*
 * מטרת העמוד:
 * הגדרת חוזה (Contract) גנרי לכל שירותי המערכת.
 * הממשק מחייב שכל סרביס שניצור יממש את פעולות ה-CRUD הבסיסיות.
 * הכל מוגדר כא-סינכרוני (Task) כדי לעבוד ביעילות מול מסד הנתונים
 * ולעמוד בדרישות הביצועים של הפרויקט.
 */