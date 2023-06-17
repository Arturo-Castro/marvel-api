using MarvelApp.Domain.Entities;

namespace MarvelApp.Infrastructure.Interfaces
{
    public interface IRescueTeamRepository
    {
        Task<RescueTeam> GetRescueTeamById(int rescueTeamId);
        Task<IEnumerable<RescueTeam>> GetAllRescueTeams();
        Task<RescueTeam> CreateRescueTeam(RescueTeam rescueTeam);
        Task<bool> RescueTeamExists(string name);
    }
}
