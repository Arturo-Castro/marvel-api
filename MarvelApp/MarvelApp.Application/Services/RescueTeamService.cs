using AutoMapper;
using MarvelApp.Application.Interfaces;
using MarvelApp.Domain;
using MarvelApp.Domain.Dtos.RescueTeam;
using MarvelApp.Domain.Entities;
using MarvelApp.Infrastructure.Interfaces;

namespace MarvelApp.Application.Services
{
    public class RescueTeamService : IRescueTeamService
    {
        private readonly IRescueTeamRepository _rescueTeamRepository;
        private readonly IMapper _mapper;
        private readonly ICharacterRepository _characterRepository;

        public RescueTeamService(IRescueTeamRepository rescueTeamRepository, IMapper mapper, ICharacterRepository characterRepository)
        {
            _rescueTeamRepository = rescueTeamRepository;
            _mapper = mapper;
            _characterRepository = characterRepository;
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

        public async Task<RescueTeamDetailDTO?> GetRescueTeamById(int rescueTeamId)
        {
            var result = await _rescueTeamRepository.GetRescueTeamById(rescueTeamId);
            if (result == null)
            {
                return null;
            }
            return _mapper.Map<RescueTeamDetailDTO>(result);
        }

        public async Task<(CreateRescueTeamDTO?, int, Enums.TeamCreationError)> CreateRescueTeam(CreateRescueTeamDTO createRescueTeamDTO)
        {
            var character = await _characterRepository.GetCharacterById(createRescueTeamDTO.CharacterId);
            if (character == null)
            {
                return (null, 0, Enums.TeamCreationError.CharacterNotFound);
            }
            else if (character.RescueTeam != null)
            {
                return (null, 0, Enums.TeamCreationError.CharacterAlreadyHasTeam);
            }

            var rescueTeamExists = await _rescueTeamRepository.RescueTeamExists(createRescueTeamDTO.Name);
            if (rescueTeamExists)
            {
                return (null, 0, Enums.TeamCreationError.TeamAlreadyExists);
            }

            var rescueTeamEntity = _mapper.Map<RescueTeam>(createRescueTeamDTO);
            rescueTeamEntity.Characters.Add(character);
            var result = await _rescueTeamRepository.CreateRescueTeam(rescueTeamEntity);
            return (_mapper.Map<CreateRescueTeamDTO>(result), result.Id, Enums.TeamCreationError.None);
        }

        public async Task<bool> EditRescueTeamName(int rescueTeamId, string newTeamName)
        {
            var rescueTeam = await _rescueTeamRepository.GetRescueTeamById(rescueTeamId);
            if (rescueTeam == null)
            {
                return false;
            }
            rescueTeam.Name = newTeamName;
            await _rescueTeamRepository.EditRescueTeamName(rescueTeam);

            return true;
        }

        public async Task<bool> DeleteRescueTeam(int rescueTeamId)
        {
            var rescueTeam = await _rescueTeamRepository.GetRescueTeamById(rescueTeamId);
            if (rescueTeam == null)
            {
                return false;
            }
            rescueTeam.Characters = null;
            await _rescueTeamRepository.DeleteRescueTeam(rescueTeam);

            return true;
        }
    }
}
