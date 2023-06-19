using FluentAssertions;
using Marvel.Api.Controllers;
using MarvelApp.Application.Interfaces;
using MarvelApp.Domain.Dtos.Character;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MarvelApp.TestApi.Systems.Controllers
{
    public class TestCharactersController
    {
        private readonly Mock<ICharacterService> _characterServiceMock;
        private readonly CharactersController _sut;

        public TestCharactersController()
        {
            _characterServiceMock = new Mock<ICharacterService>();
            _sut = new CharactersController(_characterServiceMock.Object);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetCharacterById_WhenCharacterIdIsInvalid_ShouldReturnBadRequest(int characterId)
        {
            // Act
            var result = await _sut.GetCharacterById(characterId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.Value.Should().Be("Id cannot be less than or equal to 0");
        }

        [Fact]
        public async Task GetCharacterById_WhenCharacterNotFound_ShouldReturnNotFound()
        {
            // Arrange
            _characterServiceMock.Setup(_ => _.GetCharacterById(It.IsAny<int>())).ReturnsAsync((CharacterDetailDTO?)null);

            // Act
            var result = await _sut.GetCharacterById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            notFoundResult.Value.Should().Be("No characters were found with the requested id");
        }

        [Fact]
        public async Task GetCharacterById_WhenCharacterFound_ShouldReturnOk()
        {
            // Arrange
            var characterDetailDTO = new CharacterDetailDTO();
            _characterServiceMock.Setup(_ => _.GetCharacterById(It.IsAny<int>())).ReturnsAsync(characterDetailDTO);

            // Act
            var result = await _sut.GetCharacterById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().Be(characterDetailDTO);
        }

        [Fact]
        public async Task GetAllCharacters_WhenNoCharactersFound_ShouldReturnNotFound()
        {
            // Arrange
            _characterServiceMock.Setup(_ => _.GetAllCharacters()).ReturnsAsync(new List<CharacterBaseDTO>());

            // Act
            var result = await _sut.GetAllCharacters();

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be("No characters were found");
        }

        [Fact]
        public async Task GetAllCharacters_WhenCharactersFound_ShouldReturnOk()
        {
            // Arrange
            var characters = new List<CharacterBaseDTO>
            {
                new CharacterBaseDTO(),
                new CharacterBaseDTO()
            };
            _characterServiceMock.Setup(_ => _.GetAllCharacters()).ReturnsAsync(characters);

            // Act
            var result = await _sut.GetAllCharacters();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var model = okResult.Value.Should().BeAssignableTo<List<CharacterBaseDTO>>().Subject;
            model.Should().HaveCount(2);
        }

        [Fact]
        public async Task CreateCharacter_WhenSuccessful_ShouldReturnCreatedAtActionResult()
        {
            // Arrange
            var createCharacterDTO = new CreateCharacterDTO();
            var characterId = 123;
            _characterServiceMock.Setup(_ => _.CreateCharacter(createCharacterDTO)).ReturnsAsync((createCharacterDTO, characterId));

            // Act
            var result = await _sut.CreateCharacter(createCharacterDTO);

            // Assert
            var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAtActionResult.ActionName.Should().Be(nameof(_sut.GetCharacterById));
            createdAtActionResult.RouteValues?["characterId"].Should().Be(characterId);
            createdAtActionResult.Value.Should().Be(createCharacterDTO);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task EditCharactersAttributes_WhenIdIsLessThanOrEqualToZero_ShouldReturnBadRequest(int characterId)
        {
            // Arrange
            var characterDTO = new EditCharacterBaseDTO();

            // Act
            var result = await _sut.EditCharactersAttributes(characterId, characterDTO);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Id cannot be less than or equal to 0");
        }

        [Fact]
        public async Task EditCharactersAttributes_WhenServiceReturnsFalse_ShouldReturnNotFound()
        {
            // Arrange
            var characterDTO = new EditCharacterBaseDTO();
            _characterServiceMock.Setup(_ => _.EditCharactersAttributes(characterDTO, It.IsAny<int>())).ReturnsAsync(false);

            // Act
            var result = await _sut.EditCharactersAttributes(1, characterDTO);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditCharactersAttributes_WhenServiceReturnsTrue_ShouldReturnNoContent()
        {
            // Arrange
            var characterDTO = new EditCharacterBaseDTO();
            _characterServiceMock.Setup(_ => _.EditCharactersAttributes(characterDTO, It.IsAny<int>())).ReturnsAsync(true);

            // Act
            var result = await _sut.EditCharactersAttributes(1, characterDTO);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task DeleteCharacter_WhenIdIsLessThanOrEqualToZero_ShouldReturnBadRequest(int characterId)
        {
            // Act
            var result = await _sut.DeleteCharacter(characterId);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Id cannot be less than or equal to 0");
        }

        [Fact]
        public async Task DeleteCharacter_WhenServiceReturnsFalse_ShouldReturnNotFound()
        {
            // Arrange
            _characterServiceMock.Setup(_ => _.DeleteCharacter(It.IsAny<int>())).ReturnsAsync(false);

            // Act
            var result = await _sut.DeleteCharacter(1);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteCharacter_WhenServiceReturnsTrue_ShouldReturnNoContent()
        {
            // Arrange
            _characterServiceMock.Setup(_ => _.DeleteCharacter(It.IsAny<int>())).ReturnsAsync(true);

            // Act
            var result = await _sut.DeleteCharacter(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        public async Task AssignCharacterToAnotherTeam_WhenIdIsLessThanOrEqualToZero_ShouldReturnBadRequest(int characterId, int rescueTeamId)
        {
            // Act
            var result = await _sut.AssignCharacterToAnotherTeam(characterId, rescueTeamId);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Id cannot be less than or equal to 0");
        }

        [Fact]
        public async Task AssignCharacterToAnotherTeam_WhenServiceReturnsFalse_ShouldReturnNotFound()
        {
            // Arrange
            _characterServiceMock.Setup(_ => _.AssignCharacterToATeam(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

            // Act
            var result = await _sut.AssignCharacterToAnotherTeam(1, 1);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AssignCharacterToAnotherTeam_WhenServiceReturnsTrue_ShouldReturnNoContent()
        {
            // Arrange
            _characterServiceMock.Setup(_ => _.AssignCharacterToATeam(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            // Act
            var result = await _sut.AssignCharacterToAnotherTeam(1, 1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
