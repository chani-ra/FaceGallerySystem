// Repositories/GalleryRepository.cs
using c__nRepository_2026.Interfaces;
using c__nRepository_2026.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace c__nRepository_2026.Repositories
{
    public class GalleryRepository : IGalleryRepository
    {
        private readonly IContext _context;

        public GalleryRepository(IContext context) { _context = context; }

        public async Task<Gallery> AddItemAsync(Gallery item)
        {
            item.CreatedDate = DateTime.Now;
            await _context.Galleries.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteItemAsync(int id)
        {
            var gallery = await GetByIdAsync(id);
            if (gallery != null)
            {
                _context.Galleries.Remove(gallery);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Gallery?> GetByIdAsync(int id)
        {
            return await _context.Galleries
                .AsNoTracking()
                .Include(g => g.Images)
                .Include(g => g.Character)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<List<Gallery>> GetAllAsync()
        {
            return await _context.Galleries
                .AsNoTracking()
                .Include(g => g.Character)
                .OrderByDescending(g => g.CreatedDate)
                .ToListAsync();
        }

        public async Task UpdateItemAsync(int id, Gallery item)
        {
            var existing = await _context.Galleries.FirstOrDefaultAsync(g => g.Id == id);
            if (existing != null)
            {
                item.Id = id;
                _context.Galleries.Entry(existing).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Gallery>> GetGalleriesByUserIdAsync(int userId)
        {
            return await _context.Galleries
                .AsNoTracking()
                .Where(g => g.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Gallery>> GetGalleriesByCharacterIdAsync(int characterId)
        {
            return await _context.Galleries
                .AsNoTracking()
                .Where(g => g.CharacterId == characterId)
                .ToListAsync();
        }
    }
}
