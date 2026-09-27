// Repositories/DetectedCharacterRepository.cs
using c__nRepository_2026.Interfaces;
using c__nRepository_2026.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace c__nRepository_2026.Repositories
{
    public class DetectedCharacterRepository : IDetectedRepository
    {
        private readonly IContext _context;

        public DetectedCharacterRepository(IContext context) { _context = context; }

        public async Task<DetectedCharacter> AddItemAsync(DetectedCharacter item)
        {
            item.DetectionDate = DateTime.Now;
            await _context.DetectedCharacters.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteItemAsync(int id)
        {
            var detection = await GetByIdAsync(id);
            if (detection != null)
            {
                _context.DetectedCharacters.Remove(detection);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<DetectedCharacter?> GetByIdAsync(int id)
        {
            return await _context.DetectedCharacters
                .AsNoTracking()
                .FirstOrDefaultAsync(dc => dc.Id == id);
        }

        public async Task<List<DetectedCharacter>> GetAllAsync()
        {
            return await _context.DetectedCharacters
                .AsNoTracking()
                .OrderByDescending(dc => dc.DetectionDate)
                .ToListAsync();
        }

        public async Task UpdateItemAsync(int id, DetectedCharacter item)
        {
            var existing = await _context.DetectedCharacters.FirstOrDefaultAsync(d => d.Id == id);
            if (existing != null)
            {
                item.Id = id;
                _context.DetectedCharacters.Entry(existing).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<DetectedCharacter>> GetDetectionsByImageIdAsync(int imageId)
        {
            return await _context.DetectedCharacters
                .AsNoTracking()
                .Where(dc => dc.ImageId == imageId)
                .Include(dc => dc.Character)
                .ToListAsync();
        }

        public async Task<List<DetectedCharacter>> GetDetectionsByCharacterIdAsync(int characterId)
        {
            return await _context.DetectedCharacters
                .AsNoTracking()
                .Where(dc => dc.CharacterId == characterId)
                .Include(dc => dc.Image)
                .ToListAsync();
        }
    }
}
