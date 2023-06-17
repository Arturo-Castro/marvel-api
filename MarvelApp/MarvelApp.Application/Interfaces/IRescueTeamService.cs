using MarvelApp.Domain;
using MarvelApp.Domain.Dtos.RescueTeam;

namespace MarvelApp.Application.Interfaces
{
    public interface IRescueTeamService
    {
        Task<RescueTeamDetailDTO> GetRescueTeamById(int rescueTeamId);        
        Task<IEnumerable<RescueTeamStatisticsDTO>> GetAllRescueTeamsStatistics();
        Task<(CreateRescueTeamDTO, int, Enums.TeamCreationError)> CreateRescueTeam(CreateRescueTeamDTO createRescueTeamDTO);
    }
}
