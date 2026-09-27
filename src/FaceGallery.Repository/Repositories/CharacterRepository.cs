// Repositories/CharacterRepository.cs
using c__nRepository_2026.Interfaces;
using c__nRepository_2026.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace c__nRepository_2026.Repositories
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly IContext _context;
        public CharacterRepository(IContext context) { _context = context; }

        public async Task<Character> AddItemAsync(Character item)
        {
            await _context.Characters.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteItemAsync(int id)
        {
            var character = await GetByIdAsync(id);
            if (character != null)
            {
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Character?> GetByIdAsync(int id)
        {
            return await _context.Characters
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Character>> GetAllAsync()
        {
            return await _context.Characters.AsNoTracking().ToListAsync();
        }

        public async Task UpdateItemAsync(int id, Character item)
        {
            var existing = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            if (existing != null)
            {
                item.Id = id;
                _context.Characters.Entry(existing).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Character>> SearchByNameAsync(string name)
        {
            return await _context.Characters
               .AsNoTracking()
               .Where(c => c.CharacterName.Contains(name))
               .ToListAsync();
        }
    }
}
