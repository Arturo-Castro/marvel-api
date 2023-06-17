using MarvelApp.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marvel.Api.Controllers
{
    [Route("api/rescue-teams")]
    [ApiController]
    public class RescueTeamsController : ControllerBase
    {
        private readonly IRescueTeamService _rescueTeamService;
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(RescueTeamsController));

        public RescueTeamsController(IRescueTeamService rescueTeamService)
        {
            _rescueTeamService = rescueTeamService;
        }

        [HttpGet("{rescueTeamId}")]
        public async Task<IActionResult> GetRescueTeamById([FromRoute] int rescueTeamId)
        {
            try
            {
                if (rescueTeamId <= 0)
                {
                    return BadRequest("Id cannot be less than or equal to 0");
                }
                var response = await _rescueTeamService.GetRescueTeamById(rescueTeamId);
                if (response == null)
                {
                    return NotFound("No rescue teams were found with the requested id");
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                this._logger.Error(e);
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRescueTeamsStatistics()
        {
            try
            {
                var response = await _rescueTeamService.GetAllRescueTeamsStatistics();
                if (!response.Any())
                {
                    return NotFound("No rescue teams were found");
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                this._logger.Error(e);
                return NotFound();
            }
        }
    }
}
