using MarvelApp.Domain.Dtos.Character;

namespace MarvelApp.Application.Interfaces
{
    public interface ICharacterService
    {
        Task<IEnumerable<CharacterBaseDTO>> GetAllCharacters();
        Task<CharacterDetailDTO> GetCharacterById(int characterId);
    }
}
