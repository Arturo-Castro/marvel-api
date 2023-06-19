using AutoMapper;
using FluentAssertions;
using MarvelApp.Application.Automapper;
using MarvelApp.Application.Services;
using MarvelApp.Domain;
using MarvelApp.Domain.Dtos.RescueTeam;
using MarvelApp.Domain.Entities;
using MarvelApp.Infrastructure.Interfaces;
using Moq;

namespace MarvelApp.TestApi.Systems.Services
{
    public class TestRescueTeamService
    {
        private readonly Mock<ICharacterRepository> _characterRepositoryMock;
        private readonly IMapper _mapper;
        private readonly Mock<IRescueTeamRepository> _rescueTeamRepositoryMock;
        private readonly RescueTeamService _sut;

        public TestRescueTeamService()
        {
            _characterRepositoryMock = new Mock<ICharacterRepository>();
            _rescueTeamRepositoryMock = new Mock<IRescueTeamRepository>();
            var myProfile = new MappingProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            IMapper mapper = new Mapper(configuration);
            _mapper = mapper;
            _sut = new RescueTeamService(_rescueTeamRepositoryMock.Object, _mapper, _characterRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllRescueTeamsStatistics_WhenNoRescueTeamsExist_ShouldReturnEmptyList()
        {
            // Arrange
            _rescueTeamRepositoryMock.Setup(x => x.GetAllRescueTeams()).ReturnsAsync(Enumerable.Empty<RescueTeam>());

            // Act
            var result = await _sut.GetAllRescueTeamsStatistics();

            // Assert
            result.Should().BeEmpty();
            result.Should().BeAssignableTo<IEnumerable<RescueTeamStatisticsDTO>>();
            _rescueTeamRepositoryMock.Verify(x => x.GetAllRescueTeams(), Times.Once);
        }

        [Fact]
        public async Task GetAllRescueTeamsStatistics_WhenRescueTeamsExist_ShouldReturnRescueTeamStatisticsDTOList()
        {
            // Arrange
            var rescueTeams = new List<RescueTeam>
            {
                new RescueTeam
                {
                    Name = "Team 1",
                    Characters = new List<Character>
                    {
                        new Character { Name = "Character 1", Strength = 1, Intelligence = 1, Speed = 1 },
                        new Character { Name = "Character 2", Strength = 2, Intelligence = 2, Speed = 2 }
                    }
                },
                new RescueTeam
                {
                    Name = "Team 2",
                    Characters = new List<Character>
                    {
                        new Character { Name = "Character 3", Strength = 3, Intelligence = 3, Speed = 3 },
                        new Character { Name = "Character 4", Strength = 4, Intelligence = 4, Speed = 4 }
                    }
                }
            };
            _rescueTeamRepositoryMock.Setup(x => x.GetAllRescueTeams()).ReturnsAsync(rescueTeams);

            // Act
            var result = await _sut.GetAllRescueTeamsStatistics();

            // Assert
            result.Count().Should().Be(2);
            _rescueTeamRepositoryMock.Verify(x => x.GetAllRescueTeams(), Times.Once);
            
            var team1Stats = result.First(x => x.Name == "Team 1");
            team1Stats.StrongestMember.Should().Be("Character 2");
            team1Stats.SmartestMember.Should().Be("Character 2");
            team1Stats.FastestMember.Should().Be("Character 2");

            var team2Stats = result.First(x => x.Name == "Team 2");
            team2Stats.StrongestMember.Should().Be("Character 4");
            team2Stats.SmartestMember.Should().Be("Character 4");
            team2Stats.FastestMember.Should().Be("Character 4");
        }

        [Fact]
        public async Task GetRescueTeamById_WhenRescueTeamDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var rescueTeamId = 1;
            _rescueTeamRepositoryMock.Setup(x => x.GetRescueTeamById(It.IsAny<int>())).ReturnsAsync((RescueTeam?)null);

            // Act
            var result = await _sut.GetRescueTeamById(rescueTeamId);

            // Assert
            _rescueTeamRepositoryMock.Verify(x => x.GetRescueTeamById(rescueTeamId), Times.Once);
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetRescueTeamById_WhenRescueTeamExists_ShouldReturnRescueTeamDetailDTO()
        {
            // Arrange
            var rescueTeamId = 1;
            var rescueTeam = new RescueTeam { Id = rescueTeamId };
            _rescueTeamRepositoryMock.Setup(x => x.GetRescueTeamById(It.IsAny<int>())).ReturnsAsync(rescueTeam);

            // Act
            var result = await _sut.GetRescueTeamById(rescueTeamId);

            // Assert
            result.Should().BeAssignableTo<RescueTeamDetailDTO>();
            _rescueTeamRepositoryMock.Verify(x => x.GetRescueTeamById(rescueTeamId), Times.Once);            
        }

        [Fact]
        public async Task CreateRescueTeam_WhenCharacterNotFound_ShouldReturnError()
        {
            // Arrange
            var createRescueTeamDTO = new CreateRescueTeamDTO { CharacterId = 1 };
            _characterRepositoryMock.Setup(x => x.GetCharacterById(It.IsAny<int>())).ReturnsAsync((Character?)null);

            // Act
            var (resultDTO, id, error) = await _sut.CreateRescueTeam(createRescueTeamDTO);

            // Assert
            resultDTO.Should().BeNull();
            id.Should().Be(0);
            error.Should().Be(Enums.TeamCreationError.CharacterNotFound);
        }

        [Fact]
        public async Task CreateRescueTeam_WhenCharacterAlreadyHasTeam_ShouldReturnError()
        {
            // Arrange
            var createRescueTeamDTO = new CreateRescueTeamDTO { CharacterId = 1, Name = "Rescue Team 1" };
            var character = new Character { RescueTeam = new RescueTeam() };
            _characterRepositoryMock.Setup(x => x.GetCharacterById(It.IsAny<int>())).ReturnsAsync(character);

            // Act
            var (resultDTO, id, error) = await _sut.CreateRescueTeam(createRescueTeamDTO);

            // Assert
            resultDTO.Should().BeNull();
            id.Should().Be(0);
            error.Should().Be(Enums.TeamCreationError.CharacterAlreadyHasTeam);
        }

        [Fact]
        public async Task CreateRescueTeam_WhenTeamAlreadyExists_ShouldReturnError()
        {
            // Arrange
            var createRescueTeamDTO = new CreateRescueTeamDTO { CharacterId = 1, Name = "Rescue Team 1" };
            var character = new Character { RescueTeam = null };
            _characterRepositoryMock.Setup(x => x.GetCharacterById(It.IsAny<int>())).ReturnsAsync(character);
            _rescueTeamRepositoryMock.Setup(x => x.RescueTeamExists(It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var (resultDTO, id, error) = await _sut.CreateRescueTeam(createRescueTeamDTO);

            // Assert
            resultDTO.Should().BeNull();            
            id.Should().Be(0);
            error.Should().Be(Enums.TeamCreationError.TeamAlreadyExists);
        }

        [Fact]
        public async Task CreateRescueTeam_WhenValidInputs_ShouldReturnCreatedTeam()
        {
            // Arrange
            var createRescueTeamDTO = new CreateRescueTeamDTO { CharacterId = 1, Name = "Rescue Team 1" };
            var character = new Character { RescueTeam = null };
            var rescueTeam = new RescueTeam { Id = 1, Name = "Rescue Team 1", Characters = new List<Character> { character } };
            _characterRepositoryMock.Setup(x => x.GetCharacterById(It.IsAny<int>())).ReturnsAsync(character);
            _rescueTeamRepositoryMock.Setup(x => x.RescueTeamExists(It.IsAny<string>())).ReturnsAsync(false);
            _rescueTeamRepositoryMock.Setup(x => x.CreateRescueTeam(It.IsAny<RescueTeam>())).ReturnsAsync(rescueTeam);

            // Act
            var (resultDTO, id, error) = await _sut.CreateRescueTeam(createRescueTeamDTO);

            // Assert
            resultDTO.Should().NotBeNull();
            resultDTO.Should().BeAssignableTo<CreateRescueTeamDTO>();
            id.Should().Be(rescueTeam.Id);
            error.Should().Be(Enums.TeamCreationError.None);
        }

        [Fact]
        public async Task EditRescueTeamName_WhenRescueTeamNotFound_ShouldReturnFalse()
        {
            // Arrange
            var rescueTeamId = 1;
            var newTeamName = "New Team Name";
            _rescueTeamRepositoryMock.Setup(x => x.GetRescueTeamById(It.IsAny<int>()))
                .ReturnsAsync((RescueTeam?)null);

            // Act
            var result = await _sut.EditRescueTeamName(rescueTeamId, newTeamName);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task EditRescueTeamName_WhenRescueTeamFound_ShouldEditNameAndReturnTrue()
        {
            // Arrange
            var rescueTeamId = 1;
            var newTeamName = "New Team Name";
            var rescueTeam = new RescueTeam { Id = rescueTeamId, Name = "Old Team Name" };
            _rescueTeamRepositoryMock.Setup(x => x.GetRescueTeamById(It.IsAny<int>()))
                .ReturnsAsync(rescueTeam);
            _rescueTeamRepositoryMock.Setup(x => x.EditRescueTeamName(It.IsAny<RescueTeam>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.EditRescueTeamName(rescueTeamId, newTeamName);

            // Assert
            result.Should().BeTrue();
            rescueTeam.Name.Should().Be(newTeamName);
        }

        [Fact]
        public async Task DeleteRescueTeam_WhenRescueTeamNotFound_ShouldReturnFalse()
        {
            // Arrange
            var rescueTeamId = 1;
            _rescueTeamRepositoryMock.Setup(x => x.GetRescueTeamById(It.IsAny<int>()))
                .ReturnsAsync((RescueTeam?)null);

            // Act
            var result = await _sut.DeleteRescueTeam(rescueTeamId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteRescueTeam_WhenRescueTeamFound_ShouldReturnTrue()
        {
            // Arrange
            var rescueTeamId = 1;
            var rescueTeam = new RescueTeam { Id = 1, Name = "Rescue Team 1", Characters = new List<Character>() };
            _rescueTeamRepositoryMock.Setup(x => x.GetRescueTeamById(It.IsAny<int>()))
                .ReturnsAsync(rescueTeam);
            _rescueTeamRepositoryMock.Setup(x => x.DeleteRescueTeam(It.IsAny<RescueTeam>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.DeleteRescueTeam(rescueTeamId);

            // Assert
            result.Should().BeTrue();
            rescueTeam.Characters.Should().BeNull();
        }
    }
}
