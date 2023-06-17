using MarvelApp.Domain.Entities;

namespace MarvelApp.Infrastructure.Interfaces
{
    public interface IRescueTeamRepository
    {
        Task<RescueTeam> GetRescueTeamById(int rescueTeamId);
        Task<IEnumerable<RescueTeam>> GetAllRescueTeams();        
    }
}
