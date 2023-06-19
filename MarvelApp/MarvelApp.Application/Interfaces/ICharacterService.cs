using MarvelApp.Domain.Dtos.Character;

namespace MarvelApp.Application.Interfaces
{
    public interface ICharacterService
    {
        Task<IEnumerable<CharacterBaseDTO>> GetAllCharacters();
        Task<CharacterDetailDTO?> GetCharacterById(int characterId);
        Task<(CreateCharacterDTO, int)> CreateCharacter(CreateCharacterDTO createCharacterDTO);
        Task<bool> EditCharactersAttributes(EditCharacterBaseDTO characterDTO, int characterId);
        Task<bool> DeleteCharacter(int characterId);
        Task<bool> AssignCharacterToATeam(int characterId, int RescueTeamId);
    }
}
