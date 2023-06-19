using FluentAssertions;
using MarvelApp.Domain.Entities;
using MarvelApp.Infrastructure;
using MarvelApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MarvelApp.TestApi.Systems.Repositories
{
    public class TestRescueTeamRepository : IDisposable
    {
        private readonly ApplicationContext _context;
        private readonly RescueTeamRepository _sut;

        public TestRescueTeamRepository()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationContext(options);
            _context.Database.EnsureCreated();
            _sut = new RescueTeamRepository(_context);
        }

        [Fact]
        public async Task GetAllRescueTeams_WhenRescueTeamsExist_ShouldReturnAll()
        {
            // Arrange
            var rescueTeams = new List<RescueTeam>
            {
                new RescueTeam { Name = "A", DeletedAt = null, Characters = new List<Character> { new Character { Name = "Character 1", Description = "Description 1" } } },
                new RescueTeam { Name = "B", DeletedAt = null, Characters = new List<Character> { new Character { Name = "Character 2", Description = "Description 2" } } },
                new RescueTeam { Name = "C", DeletedAt = DateTime.Now, Characters = null }
            };
            await _context.RescueTeams.AddRangeAsync(rescueTeams);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.GetAllRescueTeams();

            // Assert
            result.Should().BeAssignableTo<IEnumerable<RescueTeam>>();
            result.Should().HaveCount(2);
            result.Should().Contain(rt => rt.Name == "A");
            result.Should().Contain(rt => rt.Name == "B");
        }

        [Fact]
        public async Task GetRescueTeamById_WhenRescueTeamDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var rescueTeamId = 999;

            // Act
            var result = await _sut.GetRescueTeamById(rescueTeamId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetRescueTeamById_WhenRescueTeamExists_ShouldReturnCorrectRescueTeam()
        {
            // Arrange
            var rescueTeam = new RescueTeam
            {
                Name = "Test Team",
                Characters = new List<Character>
                {
                    new Character { Name = "Character 1", Description = "Description 1" },
                    new Character { Name = "Character 2", Description = "Description 2" }
                }
            };
            await _context.RescueTeams.AddAsync(rescueTeam);
            await _context.SaveChangesAsync();
            var rescueTeamId = rescueTeam.Id;

            // Act
            var result = await _sut.GetRescueTeamById(rescueTeamId);

            // Assert
            result.Should().NotBeNull();
            result?.Name.Should().Be("Test Team");
            result?.Characters.Should().HaveCount(2);
            result?.Characters.Should().Contain(c => c.Name == "Character 1");
            result?.Characters.Should().Contain(c => c.Name == "Character 2");
        }

        [Fact]
        public async Task CreateRescueTeam_WhenCalled_ShouldAddNewRescueTeam()
        {
            // Arrange
            var rescueTeam = new RescueTeam
            {
                Name = "Test Team",
                Characters = new List<Character>
                {
                    new Character { Name = "Character 1", Description = "Description 1" },
                    new Character { Name = "Character 2", Description = "Description 2" }
                }
            };

            // Act
            var result = await _sut.CreateRescueTeam(rescueTeam);

            // Assert
            result.Should().NotBeNull();
            result?.Name.Should().Be("Test Team");
            result?.Characters.Should().HaveCount(2);
            result?.Characters.Should().Contain(c => c.Name == "Character 1");
            result?.Characters.Should().Contain(c => c.Name == "Character 2");
            result?.CreatedAt.Should().BeCloseTo(DateTime.Now, precision: TimeSpan.FromSeconds(1));
            
            _context.RescueTeams.Should().Contain(result);
        }

        [Fact]
        public async Task RescueTeamExists_WhenRescueTeamDoesNotExist_ShouldReturnFalse()
        {            
            // Act
            var result = await _sut.RescueTeamExists("NonExistentTeam");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task RescueTeamExists_WhenRescueTeamExists_ShouldReturnTrue()
        {
            // Arrange
            var rescueTeam = new RescueTeam { Name = "Avengers" };
            await _context.RescueTeams.AddAsync(rescueTeam);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.RescueTeamExists(rescueTeam.Name);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task EditRescueTeamName_WhenRescueTeamExists_ShouldUpdateName()
        {
            // Arrange
            var rescueTeam = new RescueTeam { Name = "Avengers" };
            await _context.RescueTeams.AddAsync(rescueTeam);
            await _context.SaveChangesAsync();

            rescueTeam.Name = "New Avengers";

            // Act
            await _sut.EditRescueTeamName(rescueTeam);

            // Assert
            var result = await _context.RescueTeams.FindAsync(rescueTeam.Id);
            result.Name.Should().Be("New Avengers");
            result.UpdatedAt.Should().BeCloseTo(DateTime.Now, precision: TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task DeleteRescueTeam_WhenRescueTeamExists_ShouldSetDeletedAt()
        {
            // Arrange
            var rescueTeam = new RescueTeam { Name = "Avengers" };
            await _context.RescueTeams.AddAsync(rescueTeam);
            await _context.SaveChangesAsync();

            // Act
            await _sut.DeleteRescueTeam(rescueTeam);

            // Assert
            var result = await _context.RescueTeams.FindAsync(rescueTeam.Id);
            result.DeletedAt.Should().BeCloseTo(DateTime.Now, precision: TimeSpan.FromSeconds(1));
        }


        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
