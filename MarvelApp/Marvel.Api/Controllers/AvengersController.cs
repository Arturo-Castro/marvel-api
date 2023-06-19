using MarvelApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Marvel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvengersController : ControllerBase
    {        
        private readonly IMarvelService _marvelService;
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(AvengersController));

        public AvengersController(IMarvelService marvelService)
        {
            _marvelService = marvelService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCharactersFromNamePart([FromQuery] string nameStartsWith)
        {
            try
            {
                if (nameStartsWith.Length < 4 || string.IsNullOrEmpty(nameStartsWith))
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
            catch (Exception e)
            {
                _logger.Error(e);
                return NotFound();
            }            
        }

        [HttpGet("thanos/pdf")]
        public async Task<IActionResult> GetThanosInfoPdf()
        {
            try
            {
                byte[] pdfData = await _marvelService.GenerateThanosInfoPdf();
                return File(pdfData, "application/pdf", "thanos_report.pdf");
            }            
            catch (Exception e)
            {
                _logger.Error(e);
                return NotFound();
            }
        }
    }
}
