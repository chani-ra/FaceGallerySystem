//C#-nRepository-2026/Interfaces/IRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;

namespace c__nRepository_2026.Interfaces
{
    // הממשק הוגדר כגנרי (T) כדי לחסוך כפל קוד ולאפשר עבודה אחידה מול כל הטבלאות
    public interface IRepository<T> where T : class
    {
        Task <List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id); // סימן שאלה מציין שהאובייקט יכול להיות null
        Task<T> AddItemAsync(T item);
        Task DeleteItemAsync(int id);
        Task UpdateItemAsync(int id, T item);
    }
}
