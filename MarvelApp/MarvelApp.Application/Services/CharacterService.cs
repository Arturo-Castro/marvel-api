using AutoMapper;
using MarvelApp.Application.Interfaces;
using MarvelApp.Domain.Dtos.Character;
using MarvelApp.Infrastructure.Interfaces;

namespace MarvelApp.Application.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IMapper _mapper;

        public CharacterService(ICharacterRepository characterRepository, IMapper mapper)
        {
            _characterRepository = characterRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CharacterBaseDTO>> GetAllCharacters()
        {
            var result = await _characterRepository.GetAllCharacters();
            var response = _mapper.Map<IEnumerable<CharacterBaseDTO>>(result);
            return response;
        }

        public async Task<CharacterDetailDTO> GetCharacterById(int characterId)
        {
            var result = await _characterRepository.GetCharacterById(characterId);
            var response = _mapper.Map<CharacterDetailDTO>(result);
            return response;
        }
    }
}
