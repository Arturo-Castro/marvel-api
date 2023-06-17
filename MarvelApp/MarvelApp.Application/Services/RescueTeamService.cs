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

        public Task<RescueTeamStatisticsDTO> GetAllRescueTeamsStatistics()
        {
            throw new NotImplementedException();
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
