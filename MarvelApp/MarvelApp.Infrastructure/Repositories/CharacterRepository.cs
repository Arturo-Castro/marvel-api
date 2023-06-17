using MarvelApp.Domain.Entities;
using MarvelApp.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarvelApp.Infrastructure.Repositories
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly ApplicationContext _context;

        public CharacterRepository(ApplicationContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<Character>> GetAllCharacters()
        {
            var result = await _context.Characters
                .Where(c => c.DeletedAt == null)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return result;
        }

        public async Task<Character> GetCharacterById(int characterId)
        {
            var result = await _context.Characters
                .Include(rt => rt.RescueTeam)
                .FirstOrDefaultAsync(c => c.Id == characterId && c.DeletedAt == null);
            return result;         
        }

        public async Task<Character> CreateCharacter(Character character)
        {
            character.CreatedAt = DateTime.Now;
            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();
            return character;
        }

        public async Task EditCharactersAttributes(Character character)
        {
            character.UpdatedAt = DateTime.Now;
            _context.Characters.Update(character);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCharacter(Character character)
        {
            character.DeletedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }
}
