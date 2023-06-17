using MarvelApp.Application.Interfaces;
using MarvelApp.Domain;
using MarvelApp.Domain.Dtos.RescueTeam;
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

        [HttpPost]
        public async Task<IActionResult> CreateRescueTeam([FromBody] CreateRescueTeamDTO createRescueTeamDTO)
        {
            var (rescueTeam, id, errorCode) = await _rescueTeamService.CreateRescueTeam(createRescueTeamDTO);
            if (errorCode == Enums.TeamCreationError.CharacterNotFound)
            {
                return NotFound("No characters were found to assign as leader of the team");                
            }
            else if (errorCode == Enums.TeamCreationError.CharacterAlreadyHasTeam)
            {
                return BadRequest("That character already has a team assigned");
            }
            else if (errorCode == Enums.TeamCreationError.TeamAlreadyExists)
            {
                return BadRequest("That team already exists");
            }

            return CreatedAtAction(nameof(GetRescueTeamById), new { rescueTeamId = id }, createRescueTeamDTO);
        }

        [HttpPut("{rescueTeamId}")]
        public async Task<IActionResult> EditRescueTeamName([FromRoute] int rescueTeamId, [FromQuery] string newTeamName)
        {            
            try
            {
                if (rescueTeamId <= 0)
                {
                    return BadRequest("Id cannot be less than or equal to 0");
                }

                var success = await _rescueTeamService.EditRescueTeamName(rescueTeamId, newTeamName);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return NotFound();
            }
        }

        [HttpDelete("{rescueTeamId}")]
        public async Task<IActionResult> DeleteRescueTeam([FromRoute] int rescueTeamId)
        {
            try
            {
                if (rescueTeamId <= 0)
                {
                    return BadRequest("Id cannot be less than or equal to 0");
                }

                var success = await _rescueTeamService.DeleteRescueTeam(rescueTeamId);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception e)
            {
                this._logger.Error(e);
                return NotFound();
            }
        }
    }
}
