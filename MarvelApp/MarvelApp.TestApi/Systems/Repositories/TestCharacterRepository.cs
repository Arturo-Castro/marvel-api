using FluentAssertions;
using MarvelApp.Domain.Entities;
using MarvelApp.Infrastructure;
using MarvelApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MarvelApp.TestApi.Systems.Repositories
{
    public class TestCharacterRepository : IDisposable
    {
        private readonly ApplicationContext _context;
        private readonly CharacterRepository _sut;

        public TestCharacterRepository()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationContext(options);
            _context.Database.EnsureCreated();
            _sut = new CharacterRepository(_context);
        }

        [Fact]
        public async Task GetAllCharacters_WhenNoCharactersExist_ShouldReturnEmpty()
        {
            // Act
            var result = await _sut.GetAllCharacters();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllCharacters_WhenCharactersExist_ShouldReturnAll()
        {
            // Arrange
            var characters = new List<Character>
            {
                new Character { Name = "A", Description = "Character A", DeletedAt = null },
                new Character { Name = "B", Description = "Character B", DeletedAt = null },
                new Character { Name = "C", Description = "Character C", DeletedAt = DateTime.Now }
            };
            await _context.Characters.AddRangeAsync(characters);
            _context.SaveChanges();

            // Act
            var result = await _sut.GetAllCharacters();

            // Assert
            result.Should().BeAssignableTo<IEnumerable<Character>>();
            result.Should().HaveCount(2);
            result.Should().Contain(c => c.Name == "A");
            result.Should().Contain(c => c.Name == "B");
        }

        [Fact]
        public async Task GetCharacterById_WhenCharacterDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var characterId = 100;

            // Act
            var result = await _sut.GetCharacterById(characterId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCharacterById_WhenCharacterExists_ShouldReturnCorrectCharacter()
        {
            // Arrange
            var rescueTeam = new RescueTeam { Id = 1, Name = "Rescue Team 1" };
            await _context.RescueTeams.AddAsync(rescueTeam);
            var character = new Character { Id = 1, Name = "A", Description = "Character A", DeletedAt = null, RescueTeam = rescueTeam };
            await _context.Characters.AddAsync(character);
            _context.SaveChanges();

            // Act
            var result = await _sut.GetCharacterById(character.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Character>();
            result.Id.Should().Be(character.Id);
            result.Name.Should().Be(character.Name);
            result.RescueTeam.Should().NotBeNull();
            result.RescueTeam.Name.Should().Be(rescueTeam.Name);
        }

        [Fact]
        public async Task CreateCharacter_ShouldAddCharacterToDatabase()
        {
            // Arrange
            var character = new Character { Name = "A", Description = "Character A" };

            // Act
            var result = await _sut.CreateCharacter(character);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Character>();
            result.Id.Should().BePositive();
            result.CreatedAt.Should().BeCloseTo(DateTime.Now, precision: TimeSpan.FromSeconds(1));

            // Verify the character is in the database
            var checkCharacter = await _context.Characters.FindAsync(result.Id);
            checkCharacter.Should().NotBeNull();
            checkCharacter.Name.Should().Be(character.Name);
            checkCharacter.Description.Should().Be(character.Description);
        }

        [Fact]
        public async Task EditCharactersAttributes_ShouldUpdateCharacterAttributes()
        {
            // Arrange
            var character = new Character { Name = "A", Description = "Character A" };
            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();

            character.Name = "B";
            character.Description = "Character B";

            // Act
            await _sut.EditCharactersAttributes(character);

            // Assert
            var result = await _context.Characters.FindAsync(character.Id);
            result.Should().NotBeNull();
            result.Name.Should().Be("B");
            result.Description.Should().Be("Character B");
            result.UpdatedAt.Should().BeCloseTo(DateTime.Now, precision: TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task DeleteCharacter_WhenCharacterExists_ShouldMarkAsDeleted()
        {
            // Arrange
            var character = new Character { Name = "A", Description = "Character A" };
            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();

            // Act
            await _sut.DeleteCharacter(character);

            // Assert
            var result = await _context.Characters.FindAsync(character.Id);
            result.Should().NotBeNull();
            result.DeletedAt.Should().BeCloseTo(DateTime.Now, precision: TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task AssignCharacterToATeam_WhenCharacterExists_ShouldUpdateCharacter()
        {
            // Arrange
            var rescueTeam = new RescueTeam { Name = "Team A" };
            await _context.RescueTeams.AddAsync(rescueTeam);

            var character = new Character { Name = "A", Description = "Character A", RescueTeam = rescueTeam };
            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();

            int newTeamId = 1000;
            character.RescueTeamId = newTeamId;

            // Act
            await _sut.AssignCharacterToATeam(character);

            // Assert
            var result = await _context.Characters.FindAsync(character.Id);
            result.Should().NotBeNull();
            result.RescueTeamId.Should().Be(newTeamId);
            result.UpdatedAt.Should().BeCloseTo(DateTime.Now, precision: TimeSpan.FromSeconds(1));
        }


        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
