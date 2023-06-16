using MarvelApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Marvel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvengersController : ControllerBase
    {        
        private readonly IMarvelService _marvelService;

        public AvengersController(IMarvelService marvelService)
        {
            _marvelService = marvelService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCharactersFromNamePart([FromQuery] string nameStartsWith)
        {    
            if(nameStartsWith.Length < 4 || string.IsNullOrEmpty(nameStartsWith))
            {
                return BadRequest("Name cannot have less than 4 letters or be null");
            }
            var response = await _marvelService.GetCharactersFromNamePart(nameStartsWith);
            if (!response.Any())
            {
                return NotFound("No characters were found matching the criteria");
            }
            return Ok(response);
        }
    }
}
