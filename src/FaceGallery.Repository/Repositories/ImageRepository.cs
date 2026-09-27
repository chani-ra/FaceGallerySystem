// Repositories/ImageRepository.cs
using c__nRepository_2026.Interfaces;
using c__nRepository_2026.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace c__nRepository_2026.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly IContext _context;

        public ImageRepository(IContext context) { _context = context; }

        public async Task<Image> AddItemAsync(Image item)
        {
            await _context.Images.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteItemAsync(int id)
        {
            var image = await GetByIdAsync(id);
            if (image != null)
            {
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Image?> GetByIdAsync(int id)
        {
            return await _context.Images
                .AsNoTracking()
                .Include(i => i.DetectedCharacters)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<Image>> GetAllAsync()
        {
            return await _context.Images.AsNoTracking().ToListAsync();
        }

        public async Task UpdateItemAsync(int id, Image item)
        {
            var existing = await _context.Images.FirstOrDefaultAsync(i => i.Id == id);
            if (existing != null)
            {
                item.Id = id;
                _context.Images.Entry(existing).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Image>> GetImagesByGalleryIdAsync(int galleryId)
        {
            return await _context.Images
                .AsNoTracking()
                .Where(i => i.GalleryId == galleryId)
                .ToListAsync();
        }

        public async Task<List<Image>> GetImagesByUserIdAsync(int userId)
        {
            return await _context.Images
               .AsNoTracking()
               .Where(i => i.UserId == userId)
               .ToListAsync();
        }
    }
}
