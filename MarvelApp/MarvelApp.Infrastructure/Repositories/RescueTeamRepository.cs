using MarvelApp.Domain.Entities;
using MarvelApp.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarvelApp.Infrastructure.Repositories
{
    public class RescueTeamRepository : IRescueTeamRepository
    {
        private readonly ApplicationContext _context;

        public RescueTeamRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RescueTeam>> GetAllRescueTeams()
        {
            var result = await _context.RescueTeams
                .Where(rt => rt.DeletedAt == null)
                .Include(rt => rt.Characters)
                .OrderBy(rt => rt.Name)
                .ToListAsync();
            return result;
        }

        public async Task<RescueTeam> GetRescueTeamById(int rescueTeamId)
        {
            var result = await _context.RescueTeams
                .Include(rt => rt.Characters)
                .FirstOrDefaultAsync(rt => rt.Id == rescueTeamId && rt.DeletedAt == null);                
            return result;
        }
    }
}
