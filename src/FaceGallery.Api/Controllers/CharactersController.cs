using c__service_2026.Dto;
using c__service_2026.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FaceGallery.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharactersController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharactersController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CharacterDto>>> GetAll()
        {
            return Ok(await _characterService.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CharacterDto>> GetById(int id)
        {
            var character = await _characterService.GetByIdAsync(id);
            if (character == null) return NotFound();
            return Ok(character);
        }

        [HttpPost]
        public async Task<ActionResult<CharacterDto>> Create([FromBody] CharacterDto dto)
        {
            var created = await _characterService.AddItemAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
    }
}
