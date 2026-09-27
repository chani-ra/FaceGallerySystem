//C#-nRepository-2026/Interfaces/IContext.cs
using c__nRepository_2026.Entities;
using Microsoft.EntityFrameworkCore;

namespace c__nRepository_2026.Interfaces
{
    public interface IContext
    {
        DbSet<Character> Characters { get; set; }
        DbSet<DetectedCharacter> DetectedCharacters { get; set; }
        DbSet<Gallery> Galleries { get; set; }
        DbSet<Image> Images { get; set; }
        DbSet<User> Users { get; set; } 

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
