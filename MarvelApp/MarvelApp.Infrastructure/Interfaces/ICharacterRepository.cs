using MarvelApp.Domain.Entities;

namespace MarvelApp.Infrastructure.Interfaces
{
    public interface ICharacterRepository
    {
        Task<IEnumerable<Character>> GetAllCharacters();
        Task<Character> GetCharacterById(int characterId);
        Task<Character> CreateCharacter(Character character);
    }
}
