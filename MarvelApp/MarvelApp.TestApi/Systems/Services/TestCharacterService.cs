using AutoMapper;
using FluentAssertions;
using MarvelApp.Application.Automapper;
using MarvelApp.Application.Services;
using MarvelApp.Domain.Dtos.Character;
using MarvelApp.Domain.Entities;
using MarvelApp.Infrastructure.Interfaces;
using Moq;

namespace MarvelApp.TestApi.Systems.Services
{
    public class TestCharacterService
    {
        private readonly Mock<ICharacterRepository> _characterRepositoryMock;
        private readonly IMapper _mapper;
        private readonly Mock<IRescueTeamRepository> _rescueTeamRepositoryMock;
        private readonly CharacterService _sut;

        public TestCharacterService()
        {
            _characterRepositoryMock = new Mock<ICharacterRepository>();
            _rescueTeamRepositoryMock = new Mock<IRescueTeamRepository>();
            var myProfile = new MappingProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            IMapper mapper = new Mapper(configuration);
            _mapper = mapper;
            _sut = new CharacterService(_characterRepositoryMock.Object, _mapper, _rescueTeamRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllCharacters_WhenNoCharactersExist_ShouldReturnEmptyList()
        {
            // Arrange
            _characterRepositoryMock.Setup(x => x.GetAllCharacters())
                .ReturnsAsync(Enumerable.Empty<Character>());

            // Act
            var result = await _sut.GetAllCharacters();

            // Assert
            result.Should().BeEmpty();
            result.Should().BeAssignableTo<IEnumerable<CharacterBaseDTO>>();
        }

        [Fact]
        public async Task GetAllCharacters_WhenCharactersExist_ShouldReturnCharacterBaseDTOList()
        {
            // Arrange
            var characters = new List<Character> { new Character(), new Character() };
            _characterRepositoryMock.Setup(x => x.GetAllCharacters())
                .ReturnsAsync(characters);            

            // Act
            var result = await _sut.GetAllCharacters();

            // Assert            
            result.Should().BeAssignableTo<IEnumerable<CharacterBaseDTO>>();
            _characterRepositoryMock.Verify(x => x.GetAllCharacters(), Times.Once);
        }

        [Fact]
        public async Task GetCharacterById_WhenCharacterDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            int characterId = 1;
            _characterRepositoryMock.Setup(x => x.GetCharacterById(It.IsAny<int>())).ReturnsAsync((Character?)null);

            // Act
            var result = await _sut.GetCharacterById(characterId);

            // Assert
            result.Should().BeNull();
            _characterRepositoryMock.Verify(x => x.GetCharacterById(characterId), Times.Once);
        }

        [Fact]
        public async Task GetCharacterById_WhenCharacterExists_ShouldReturnCharacterDetailDTO()
        {
            // Arrange
            int characterId = 1;
            var character = new Character { Id = characterId };
            _characterRepositoryMock.Setup(x => x.GetCharacterById(It.IsAny<int>())).ReturnsAsync(character);

            // Act
            var result = await _sut.GetCharacterById(characterId);

            // Assert
            result.Should().BeAssignableTo<CharacterDetailDTO>();
            _characterRepositoryMock.Verify(x => x.GetCharacterById(characterId), Times.Once);
        }

        [Fact]
        public async Task CreateCharacter_WhenCalled_ShouldReturnCreateCharacterDTOAndId()
        {
            // Arrange
            var createCharacterDto = new CreateCharacterDTO();
            var character = new Character { Id = 1 };

            _characterRepositoryMock.Setup(x => x.CreateCharacter(It.IsAny<Character>())).ReturnsAsync(character);

            // Act
            var (resultCharacterDto, resultId) = await _sut.CreateCharacter(createCharacterDto);

            // Assert
            resultCharacterDto.Should().BeEquivalentTo(createCharacterDto);
            resultId.Should().Be(character.Id);
            _characterRepositoryMock.Verify(x => x.CreateCharacter(It.IsAny<Character>()), Times.Once);
        }

        [Fact]
        public async Task EditCharactersAttributes_WhenCharacterNotFound_ShouldReturnFalse()
        {
            // Arrange
            int characterId = 1;
            var characterDto = new EditCharacterBaseDTO();

            _characterRepositoryMock.Setup(x => x.GetCharacterById(It.IsAny<int>())).ReturnsAsync((Character?)null);

            // Act
            var result = await _sut.EditCharactersAttributes(characterDto, characterId);

            // Assert
            result.Should().BeFalse();
            _characterRepositoryMock.Verify(x => x.GetCharacterById(characterId), Times.Once);
            _characterRepositoryMock.Verify(x => x.EditCharactersAttributes(It.IsAny<Character>()), Times.Never);
        }

        [Fact]
        public async Task EditCharactersAttributes_WhenCharacterFound_ShouldReturnTrue()
        {
            // Arrange
            int characterId = 1;
            var characterDto = new EditCharacterBaseDTO();
            var character = new Character();

            _characterRepositoryMock.Setup(x => x.GetCharacterById(It.IsAny<int>())).ReturnsAsync(character);
            _characterRepositoryMock.Setup(x => x.EditCharactersAttributes(It.IsAny<Character>())).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.EditCharactersAttributes(characterDto, characterId);

            // Assert
            result.Should().BeTrue();
            _characterRepositoryMock.Verify(x => x.GetCharacterById(characterId), Times.Once);
            _characterRepositoryMock.Verify(x => x.EditCharactersAttributes(character), Times.Once);
        }

        [Fact]
        public async Task DeleteCharacter_WhenCharacterNotFound_ShouldReturnFalse()
        {
            // Arrange
            int characterId = 1;
            _characterRepositoryMock.Setup(x => x.GetCharacterById(It.IsAny<int>())).ReturnsAsync((Character?)null);

            // Act
            var result = await _sut.DeleteCharacter(characterId);

            // Assert
            result.Should().BeFalse();
            _characterRepositoryMock.Verify(x => x.GetCharacterById(characterId), Times.Once);
            _characterRepositoryMock.Verify(x => x.DeleteCharacter(It.IsAny<Character>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCharacter_WhenCharacterFound_ShouldReturnTrue()
        {
            // Arrange
            int characterId = 1;
            var character = new Character();

            _characterRepositoryMock.Setup(x => x.GetCharacterById(characterId)).ReturnsAsync(character);
            _characterRepositoryMock.Setup(x => x.DeleteCharacter(character)).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.DeleteCharacter(characterId);

            // Assert
            result.Should().BeTrue();
            _characterRepositoryMock.Verify(x => x.GetCharacterById(characterId), Times.Once);
            _characterRepositoryMock.Verify(x => x.DeleteCharacter(character), Times.Once);
        }

        [Fact]
        public async Task AssignCharacterToATeam_WhenCharacterNotFound_ShouldReturnFalse()
        {
            // Arrange
            int characterId = 1;
            int rescueTeamId = 2;
            _characterRepositoryMock.Setup(x => x.GetCharacterById(It.IsAny<int>())).ReturnsAsync((Character?)null);

            // Act
            var result = await _sut.AssignCharacterToATeam(characterId, rescueTeamId);

            // Assert
            result.Should().BeFalse();
            _characterRepositoryMock.Verify(x => x.GetCharacterById(characterId), Times.Once);
            _characterRepositoryMock.Verify(x => x.AssignCharacterToATeam(It.IsAny<Character>()), Times.Never);
        }

        [Fact]
        public async Task AssignCharacterToATeam_WhenRescueTeamNotFound_ShouldReturnFalse()
        {
            // Arrange
            int characterId = 1;
            int rescueTeamId = 2;
            var character = new Character();

            _characterRepositoryMock.Setup(x => x.GetCharacterById(It.IsAny<int>())).ReturnsAsync(character);
            _rescueTeamRepositoryMock.Setup(x => x.GetRescueTeamById(It.IsAny<int>())).ReturnsAsync((RescueTeam?)null);

            // Act
            var result = await _sut.AssignCharacterToATeam(characterId, rescueTeamId);

            // Assert
            result.Should().BeFalse();
            _characterRepositoryMock.Verify(x => x.GetCharacterById(characterId), Times.Once);
            _characterRepositoryMock.Verify(x => x.AssignCharacterToATeam(It.IsAny<Character>()), Times.Never);
        }

        [Fact]
        public async Task AssignCharacterToATeam_WhenBothFound_ShouldReturnTrue()
        {
            // Arrange
            int characterId = 1;
            int rescueTeamId = 2;
            var character = new Character();
            var rescueTeam = new RescueTeam();

            _characterRepositoryMock.Setup(x => x.GetCharacterById(It.IsAny<int>())).ReturnsAsync(character);
            _rescueTeamRepositoryMock.Setup(x => x.GetRescueTeamById(It.IsAny<int>())).ReturnsAsync(rescueTeam);
            _characterRepositoryMock.Setup(x => x.AssignCharacterToATeam(It.IsAny<Character>())).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.AssignCharacterToATeam(characterId, rescueTeamId);

            // Assert
            result.Should().BeTrue();
            _characterRepositoryMock.Verify(x => x.GetCharacterById(characterId), Times.Once);
            _characterRepositoryMock.Verify(x => x.AssignCharacterToATeam(character), Times.Once);
        }
    }
}
