using AutoMapper;
using MarvelApp.Application.Interfaces;
using MarvelApp.Domain.Dtos.RescueTeam;
using MarvelApp.Infrastructure.Interfaces;

namespace MarvelApp.Application.Services
{
    public class RescueTeamService : IRescueTeamService
    {
        private readonly IRescueTeamRepository _rescueTeamRepository;
        private readonly IMapper _mapper;

        public RescueTeamService(IRescueTeamRepository rescueTeamRepository, IMapper mapper)
        {
            _rescueTeamRepository = rescueTeamRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RescueTeamStatisticsDTO>> GetAllRescueTeamsStatistics()
        {
            var result = await _rescueTeamRepository.GetAllRescueTeams();
            if(!result.Any())
            {
                return Enumerable.Empty<RescueTeamStatisticsDTO>();
            }
            var rescueTeamStatisticDTO = result                
                .Select(t => new RescueTeamStatisticsDTO
                {
                    Name = t.Name,
                    MembersCount = t.Characters.Count,
                    StrongestMember = t.Characters.OrderByDescending(m => m.Strength).FirstOrDefault()?.Name,
                    SmartestMember = t.Characters.OrderByDescending(m => m.Intelligence).FirstOrDefault()?.Name,
                    FastestMember = t.Characters.OrderByDescending(m => m.Speed).FirstOrDefault()?.Name
                });
            return rescueTeamStatisticDTO;
        }

        public async Task<RescueTeamDetailDTO> GetRescueTeamById(int rescueTeamId)
        {
            var result = await _rescueTeamRepository.GetRescueTeamById(rescueTeamId);
            if (result == null)
            {
                return null;
            }
            return _mapper.Map<RescueTeamDetailDTO>(result);
        }
    }
}
