using MarvelApp.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marvel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharactersController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharactersController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet("{characterId}")]
        public async Task<IActionResult> GetCharacterById([FromRoute] int characterId)
        {
            if (characterId <= 0)
            {
                return BadRequest("Id cannot be less than or equal to 0");
            }

            var response = await _characterService.GetCharacterById(characterId);
            if(response == null)
            {
                return NotFound("No characters were found with the requested id");
            }
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCharacters()
        {
            var response = await _characterService.GetAllCharacters();
            if (!response.Any())
            {
                return NotFound("No characters were found");
            }
            return Ok(response);
        }
    }
}
