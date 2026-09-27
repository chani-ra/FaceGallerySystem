// הנתיב המלא: C#-2026-service/Services/DetectedCharacterService.cs
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
    public class DetectedCharacterService : IDetectedService
    {
        private readonly IDetectedRepository _detectionRepository;
        private readonly ICharacterRepository _characterRepository;
        private readonly IMapper _mapper;

        public DetectedCharacterService(IDetectedRepository detectionRepository, ICharacterRepository characterRepository, IMapper mapper)
        {
            _detectionRepository = detectionRepository;
            _characterRepository = characterRepository;
            _mapper = mapper;
        }

        public async Task<List<DetectedCharacterDto>> GetAllAsync()
        {
            var detections = await _detectionRepository.GetAllAsync();
            return await MapToDTOListAsync(detections);
        }

        public async Task<DetectedCharacterDto> GetByIdAsync(int id)
        {
            var detection = await _detectionRepository.GetByIdAsync(id);
            return detection == null ? null : await MapToDTOAsync(detection);
        }

        public async Task<DetectedCharacterDto> AddItemAsync(DetectedCharacterDto item)
        {
            var entity = _mapper.Map<DetectedCharacter>(item);
            entity.DetectionDate = DateTime.Now; // קביעת זמן הזיהוי לעכשיו

            var result = await _detectionRepository.AddItemAsync(entity);
            return await MapToDTOAsync(result);
        }

        public async Task UpdateItemAsync(int id, DetectedCharacterDto item)
        {
            var entity = _mapper.Map<DetectedCharacter>(item);
            await _detectionRepository.UpdateItemAsync(id, entity);
        }

        public async Task DeleteItemAsync(int id)
        {
            await _detectionRepository.DeleteItemAsync(id);
        }

        // --- לוגיקה ייחודית לזיהויים ---

        public async Task<List<DetectedCharacterDto>> GetByImageAsync(int imageId)
        {
            var detections = await _detectionRepository.GetDetectionsByImageIdAsync(imageId);
            return await MapToDTOListAsync(detections);
        }

        public async Task<List<DetectedCharacterDto>> GetByCharacterAsync(int characterId)
        {
            var detections = await _detectionRepository.GetDetectionsByCharacterIdAsync(characterId);
            return await MapToDTOListAsync(detections);
        }

        public async Task<List<DetectedCharacterDto>> GetHighConfidenceAsync(double minConfidence)
        {
            var all = await _detectionRepository.GetAllAsync();
            var filtered = all.Where(d => d.Confidence >= minConfidence).ToList();
            return await MapToDTOListAsync(filtered);
        }

        public async Task<Dictionary<string, object>> GetDetectionStatisticsAsync()
        {
            var all = await _detectionRepository.GetAllAsync();
            return new Dictionary<string, object>
            {
                { "TotalDetections", all.Count },
                { "AverageConfidence", all.Any() ? Math.Round(all.Average(d => d.Confidence) * 100, 2) : 0 },
                { "HighConfidenceCount", all.Count(d => d.Confidence >= 0.85) } // סופר זיהויים בטוחים במיוחד
            };
        }

        // --- פונקציות עזר למיפוי ---

        private async Task<DetectedCharacterDto> MapToDTOAsync(DetectedCharacter detection)
        {
            var dto = _mapper.Map<DetectedCharacterDto>(detection);

            // מביא את שם הדמות מהמסד כדי שהלקוח יראה את השם ולא רק מספר ID
            var character = await _characterRepository.GetByIdAsync(detection.CharacterId);
            if (character != null)
            {
                dto.CharacterName = character.CharacterName;
            }
            return dto;
        }

        private async Task<List<DetectedCharacterDto>> MapToDTOListAsync(IEnumerable<DetectedCharacter> detections)
        {
            var list = new List<DetectedCharacterDto>();
            foreach (var d in detections) list.Add(await MapToDTOAsync(d));
            return list;
        }
    }
}
