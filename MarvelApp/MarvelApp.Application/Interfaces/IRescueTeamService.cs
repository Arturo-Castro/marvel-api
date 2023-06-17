using MarvelApp.Domain.Dtos.RescueTeam;

namespace MarvelApp.Application.Interfaces
{
    public interface IRescueTeamService
    {
        Task<RescueTeamDetailDTO> GetRescueTeamById(int rescueTeamId);        
        Task<IEnumerable<RescueTeamStatisticsDTO>> GetAllRescueTeamsStatistics();
    }
}
