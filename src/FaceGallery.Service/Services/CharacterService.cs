// הנתיב המלא: C#-2026-service/Services/CharacterService.cs
using AutoMapper;
using c__nRepository_2026.Entities;
using c__nRepository_2026.Interfaces;
using c__service_2026.Dto;
using c__service_2026.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace c__service_2026.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IDetectedRepository _detectionRepository;
        private readonly IMapper _mapper;

        public CharacterService(ICharacterRepository characterRepository, IDetectedRepository detectionRepository, IMapper mapper)
        {
            _characterRepository = characterRepository;
            _detectionRepository = detectionRepository;
            _mapper = mapper;
        }

        public async Task<List<CharacterDto>> GetAllAsync()
        {
            var characters = await _characterRepository.GetAllAsync();
            return await MapToDTOListAsync(characters);
        }

        public async Task<CharacterDto> GetByIdAsync(int id)
        {
            var character = await _characterRepository.GetByIdAsync(id);
            return character == null ? null : await MapToDTOAsync(character);
        }

        public async Task<CharacterDto> AddItemAsync(CharacterDto item)
        {
            if (string.IsNullOrWhiteSpace(item.CharacterName)) throw new ArgumentException("שם הדמות הוא שדה חובה");

            var character = _mapper.Map<Character>(item);
            character.CreatedDate = DateTime.Now;

            var result = await _characterRepository.AddItemAsync(character);
            return await MapToDTOAsync(result);
        }

        public async Task UpdateItemAsync(int id, CharacterDto item)
        {
            if (string.IsNullOrWhiteSpace(item.CharacterName)) throw new ArgumentException("שם הדמות הוא שדה חובה");

            var existingCharacter = await _characterRepository.GetByIdAsync(id);
            if (existingCharacter == null) throw new ArgumentException("הדמות לא נמצאה במערכת");

            existingCharacter.CharacterName = item.CharacterName;
            existingCharacter.Description = item.Description;

            await _characterRepository.UpdateItemAsync(id, existingCharacter);
        }

        public async Task DeleteItemAsync(int id)
        {
            await _characterRepository.DeleteItemAsync(id);
        }

        // --- לוגיקה ייחודית מתקדמת ---

        public async Task<List<CharacterDto>> SearchByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return new List<CharacterDto>();

            var characters = await _characterRepository.SearchByNameAsync(name);
            return await MapToDTOListAsync(characters);
        }

        public async Task<List<CharacterDto>> GetTopDetectedAsync(int topCount)
        {
            // שליפת כל הזיהויים וקיבוץ לפי מזהה הדמות כדי למצוא מי הכי פופולרית
            var detections = await _detectionRepository.GetAllAsync();
            var topCharacterIds = detections
                .GroupBy(d => d.CharacterId)
                .OrderByDescending(g => g.Count())
                .Take(topCount)
                .Select(g => g.Key)
                .ToList();

            var characters = new List<CharacterDto>();
            foreach (var id in topCharacterIds)
            {
                var character = await _characterRepository.GetByIdAsync(id);
                if (character != null) characters.Add(await MapToDTOAsync(character));
            }
            return characters;
        }

        public async Task<Dictionary<string, object>> GetCharacterStatisticsAsync(int characterId)
        {
            var character = await _characterRepository.GetByIdAsync(characterId);
            if (character == null) throw new ArgumentException("הדמות לא נמצאה");

            var detections = await _detectionRepository.GetDetectionsByCharacterIdAsync(characterId);
            var avgConf = detections.Count > 0 ? detections.Average(d => d.Confidence) : 0;

            return new Dictionary<string, object>
            {
                { "CharacterId", characterId },
                { "CharacterName", character.CharacterName },
                { "TotalDetections", detections.Count },
                { "AverageConfidence", Math.Round(avgConf * 100, 2) }, // החזרת אחוזים יפים
                { "HighConfidenceCount", detections.Count(d => d.Confidence >= 0.85f) }
            };
        }

        // --- פונקציות עזר פרטיות למיפוי ---

        private async Task<CharacterDto> MapToDTOAsync(Character character)
        {
            // ממירים את הבסיס בעזרת AutoMapper
            var dto = _mapper.Map<CharacterDto>(character);

            // מוסיפים חישוב דינמי שלא קיים ב-DB: כמה פעמים הדמות הזו זוהתה
            var detections = await _detectionRepository.GetDetectionsByCharacterIdAsync(character.Id);
            dto.TotalDetections = detections.Count;

            return dto;
        }

        private async Task<List<CharacterDto>> MapToDTOListAsync(IEnumerable<Character> characters)
        {
            var list = new List<CharacterDto>();
            foreach (var c in characters)
            {
                list.Add(await MapToDTOAsync(c));
            }
            return list;
        }
    }
}
