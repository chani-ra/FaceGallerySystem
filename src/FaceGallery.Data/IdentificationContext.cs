using c__nRepository_2026.Entities;
using c__nRepository_2026.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace c__2026_data
{
    public class IdentificationContext : DbContext, IContext
    {
        public IdentificationContext(DbContextOptions<IdentificationContext> options) : base(options)
        {
        }

        public DbSet<Character> Characters { get; set; }
        public DbSet<DetectedCharacter> DetectedCharacters { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<c__nRepository_2026.Entities.Image> Images { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Avoids multiple-cascade-path errors between Gallery -> Character and
            // Gallery -> Image -> DetectedCharacter -> Character on SQL Server.
            modelBuilder.Entity<Gallery>()
                .HasOne(g => g.Character)
                .WithMany()
                .HasForeignKey(g => g.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetectedCharacter>()
                .HasOne(d => d.Character)
                .WithMany(c => c.DetectedCharacters)
                .HasForeignKey(d => d.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetectedCharacter>()
                .HasOne(d => d.Image)
                .WithMany(i => i.DetectedCharacters)
                .HasForeignKey(d => d.ImageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Image>()
                .HasOne(i => i.Gallery)
                .WithMany(g => g.Images)
                .HasForeignKey(i => i.GalleryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
