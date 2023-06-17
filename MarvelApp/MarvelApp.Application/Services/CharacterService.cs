using AutoMapper;
using MarvelApp.Application.Interfaces;
using MarvelApp.Domain.Dtos.Character;
using MarvelApp.Domain.Entities;
using MarvelApp.Infrastructure.Interfaces;

namespace MarvelApp.Application.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IMapper _mapper;
        private readonly IRescueTeamRepository _rescueTeamRepository;

        public CharacterService(ICharacterRepository characterRepository, IMapper mapper, IRescueTeamRepository rescueTeamRepository)
        {
            _characterRepository = characterRepository;
            _mapper = mapper;
            _rescueTeamRepository = rescueTeamRepository;
        }

        public async Task<IEnumerable<CharacterBaseDTO>> GetAllCharacters()
        {
            var result = await _characterRepository.GetAllCharacters();
            if(!result.Any())
            {
                return Enumerable.Empty<CharacterBaseDTO>();
            }
            var response = _mapper.Map<IEnumerable<CharacterBaseDTO>>(result);
            return response;
        }

        public async Task<CharacterDetailDTO> GetCharacterById(int characterId)
        {
            var result = await _characterRepository.GetCharacterById(characterId);
            if(result == null)
            {
                return null;
            }
            var response = _mapper.Map<CharacterDetailDTO>(result);
            return response;
        }

        public async Task<(CreateCharacterDTO, int)> CreateCharacter(CreateCharacterDTO createCharacterDTO)
        {
            var characterEntity = _mapper.Map<Character>(createCharacterDTO);
            var result = await _characterRepository.CreateCharacter(characterEntity);
            return (_mapper.Map<CreateCharacterDTO>(result), result.Id);
        }

        public async Task<bool> EditCharactersAttributes(EditCharacterBaseDTO characterDTO, int characterId)
        {
            var character = await _characterRepository.GetCharacterById(characterId);
            if(character == null)
            {
                return false;
            }
            _mapper.Map(characterDTO, character);
            await _characterRepository.EditCharactersAttributes(character);

            return true;

        }

        public async Task<bool> DeleteCharacter(int characterId)
        {
            var character = await _characterRepository.GetCharacterById(characterId);
            if (character == null)
            {
                return false;
            }
            character.RescueTeamId = null;
            await _characterRepository.DeleteCharacter(character);

            return true;
        }

        public async Task<bool> AssignCharacterToATeam(int characterId, int rescueTeamId)
        {
            var character = await _characterRepository.GetCharacterById(characterId);
            if (character == null)
            {
                return false;
            }

            var rescueTeam = await _rescueTeamRepository.GetRescueTeamById(rescueTeamId);
            if (rescueTeam == null)
            {
                return false;
            }

            character.RescueTeamId = rescueTeamId;
            await _characterRepository.AssignCharacterToATeam(character);

            return true;
        }
    }
}
