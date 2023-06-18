using FluentAssertions;
using Marvel.Api.Controllers;
using MarvelApp.Application.Interfaces;
using MarvelApp.Domain;
using MarvelApp.Domain.Dtos.RescueTeam;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MarvelApp.TestApi.Systems.Controllers
{
    public class TestRescueTeamsController
    {
        private readonly Mock<IRescueTeamService> _rescueTeamServiceMock;
        private readonly RescueTeamsController _sut;

        public TestRescueTeamsController()
        {
            _rescueTeamServiceMock = new Mock<IRescueTeamService>();
            _sut = new RescueTeamsController(_rescueTeamServiceMock.Object);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetRescueTeamById_WhenIdIsLessThanOrEqualToZero_ShouldReturnBadRequest(int rescueTeamId)
        {
            // Act
            var result = await _sut.GetRescueTeamById(rescueTeamId);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Id cannot be less than or equal to 0");
        }

        [Fact]
        public async Task GetRescueTeamById_WhenServiceReturnsNull_ShouldReturnNotFound()
        {
            // Arrange
            _rescueTeamServiceMock.Setup(_ => _.GetRescueTeamById(It.IsAny<int>())).ReturnsAsync((RescueTeamDetailDTO?)null);

            // Act
            var result = await _sut.GetRescueTeamById(1);

            // Assert
            var notFoundresult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundresult.Value.Should().Be("No rescue teams were found with the requested id");
        }

        [Fact]
        public async Task GetRescueTeamById_WhenRescueTeamFound_ShouldReturnOk()
        {
            // Arrange
            var expectedRescueTeam = new RescueTeamDetailDTO();
            _rescueTeamServiceMock.Setup(_ => _.GetRescueTeamById(It.IsAny<int>())).ReturnsAsync(expectedRescueTeam);

            // Act
            var result = await _sut.GetRescueTeamById(1);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var model = okResult.Value.Should().BeAssignableTo<RescueTeamDetailDTO>().Subject;
            model.Should().BeSameAs(expectedRescueTeam);
        }

        [Fact]
        public async Task GetAllRescueTeamsStatistics_WhenRescueTeamNotFound_ShouldReturnNotFound()
        {
            // Arrange
            _rescueTeamServiceMock.Setup(_ => _.GetAllRescueTeamsStatistics()).ReturnsAsync(new List<RescueTeamStatisticsDTO>());

            // Act
            var result = await _sut.GetAllRescueTeamsStatistics();

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be("No rescue teams were found");
        }

        [Fact]
        public async Task GetAllRescueTeamsStatistics_WhenRescueTeamsStatisticsAreShown_ShouldReturnOk()
        {
            // Arrange
            var expectedRescueTeams = new List<RescueTeamStatisticsDTO> { new RescueTeamStatisticsDTO() };
            _rescueTeamServiceMock.Setup(_ => _.GetAllRescueTeamsStatistics()).ReturnsAsync(expectedRescueTeams);

            // Act
            var result = await _sut.GetAllRescueTeamsStatistics();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var model = okResult.Value.Should().BeAssignableTo<IEnumerable<RescueTeamStatisticsDTO>>().Subject;
            model.Should().BeEquivalentTo(expectedRescueTeams);
        }

        [Fact]
        public async Task CreateRescueTeam_WhenCharacterNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var rescueTeamDTO = new CreateRescueTeamDTO();
            _rescueTeamServiceMock.Setup(_ => _.CreateRescueTeam(rescueTeamDTO))
                .ReturnsAsync((null, 0, Enums.TeamCreationError.CharacterNotFound));

            // Act
            var result = await _sut.CreateRescueTeam(rescueTeamDTO);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be("No characters were found to assign as leader of the team");
        }

        [Fact]
        public async Task CreateRescueTeam_WhenCharacterAlreadyHasTeam_ShouldReturnBadRequest()
        {
            // Arrange
            var rescueTeamDTO = new CreateRescueTeamDTO();
            _rescueTeamServiceMock.Setup(_ => _.CreateRescueTeam(rescueTeamDTO))
                .ReturnsAsync((null, 0, Enums.TeamCreationError.CharacterAlreadyHasTeam));

            // Act
            var result = await _sut.CreateRescueTeam(rescueTeamDTO);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("That character already has a team assigned");
        }

        [Fact]
        public async Task CreateRescueTeam_WhenTeamAlreadyExists_ShouldReturnBadRequest()
        {
            // Arrange
            var rescueTeamDTO = new CreateRescueTeamDTO();
            _rescueTeamServiceMock.Setup(_ => _.CreateRescueTeam(rescueTeamDTO))
                .ReturnsAsync((null, 0, Enums.TeamCreationError.TeamAlreadyExists));

            // Act
            var result = await _sut.CreateRescueTeam(rescueTeamDTO);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("That team already exists");
        }

        [Fact]
        public async Task CreateRescueTeam_WhenSuccessful_ShouldReturnCreatedAtActionResult()
        {
            // Arrange
            var rescueTeamDTO = new CreateRescueTeamDTO();
            _rescueTeamServiceMock.Setup(_ => _.CreateRescueTeam(rescueTeamDTO))
                .ReturnsAsync((new CreateRescueTeamDTO(), 1, Enums.TeamCreationError.None));

            // Act
            var result = await _sut.CreateRescueTeam(rescueTeamDTO);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdAtResult = result as CreatedAtActionResult;
            createdAtResult?.ActionName.Should().Be(nameof(_sut.GetRescueTeamById));
            createdAtResult?.RouteValues?["rescueTeamId"].Should().Be(1);
            createdAtResult?.Value.Should().Be(rescueTeamDTO);
        }

        [Fact]
        public async Task EditRescueTeamName_WhenIdIsLessThanOrEqualToZero_ShouldReturnBadRequest()
        {
            // Arrange
            int rescueTeamId = 0;
            string newTeamName = "New Team Name";

            // Act
            var result = await _sut.EditRescueTeamName(rescueTeamId, newTeamName);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Id cannot be less than or equal to 0");
        }

        [Fact]
        public async Task EditRescueTeamName_WhenRescueTeamIsNotFound_ShouldReturnNotFound()
        {
            // Arrange
            int rescueTeamId = 1;
            string newTeamName = "New Team Name";
            _rescueTeamServiceMock.Setup(_ => _.EditRescueTeamName(rescueTeamId, newTeamName)).ReturnsAsync(false);

            // Act
            var result = await _sut.EditRescueTeamName(rescueTeamId, newTeamName);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditRescueTeamName_WhenSuccessful_ShouldReturnNoContent()
        {
            // Arrange
            int rescueTeamId = 1;
            string newTeamName = "New Team Name";
            _rescueTeamServiceMock.Setup(_ => _.EditRescueTeamName(rescueTeamId, newTeamName)).ReturnsAsync(true);

            // Act
            var result = await _sut.EditRescueTeamName(rescueTeamId, newTeamName);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task DeleteRescueTeam_WhenIdIsLessThanOrEqualToZero_ShouldReturnBadRequest(int rescueTeamId)
        {
            // Act
            var result = await _sut.DeleteRescueTeam(rescueTeamId);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Id cannot be less than or equal to 0");
        }

        [Fact]
        public async Task DeleteRescueTeam_WhenNotSuccessful_ShouldReturnNotFound()
        {
            // Arrange
            int rescueTeamId = 1;
            _rescueTeamServiceMock.Setup(_ => _.DeleteRescueTeam(rescueTeamId)).ReturnsAsync(false);

            // Act
            var result = await _sut.DeleteRescueTeam(rescueTeamId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteRescueTeam_WhenSuccessful_ShouldReturnNoContent()
        {
            // Arrange
            int rescueTeamId = 1;
            _rescueTeamServiceMock.Setup(_ => _.DeleteRescueTeam(rescueTeamId)).ReturnsAsync(true);

            // Act
            var result = await _sut.DeleteRescueTeam(rescueTeamId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
