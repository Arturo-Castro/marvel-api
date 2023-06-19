using FluentAssertions;
using Marvel.Api.Controllers;
using MarvelApp.Application.Interfaces;
using MarvelApp.Domain.Dtos.MarvelApiRestService;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MarvelApp.TestApi.Systems.Controllers
{
    public class TestAvengersController
    {
        private readonly Mock<IMarvelService> _marvelServiceMock;
        private readonly AvengersController _sut;

        public TestAvengersController()
        {
            _marvelServiceMock = new Mock<IMarvelService>();
            _sut = new AvengersController(_marvelServiceMock.Object);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        public async Task GetCharactersFromNamePart_WhenNameStartsWithIsInvalid_ShouldReturnBadRequest(string nameStartsWith)
        {
            // Act
            var result = await _sut.GetCharactersFromNamePart(nameStartsWith);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.Value.Should().Be("Name cannot have less than 4 letters or be null");
        }

        [Fact]
        public async Task GetCharactersFromNamePart_WhenNoCharactersFound_ShouldReturnNotFound()
        {
            // Arrange
            string nameStartsWith = "abcdg2,4";
            _marvelServiceMock.Setup(_ => _.GetCharactersFromNamePart(nameStartsWith))
                .ReturnsAsync(new List<MarvelResponseDTO.CharacterSeriesDTO>());

            // Act
            var result = await _sut.GetCharactersFromNamePart(nameStartsWith);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            notFoundResult.Value.Should().Be("No characters were found matching the criteria");
        }

        [Fact]
        public async Task GetCharactersFromNamePart_WhenCharactersFound_ShouldReturnOk()
        {
            // Arrange
            string nameStartsWith = "abcd";
            var characterSeriesDTO = new List<MarvelResponseDTO.CharacterSeriesDTO> { new MarvelResponseDTO.CharacterSeriesDTO() };
            _marvelServiceMock.Setup(service => service.GetCharactersFromNamePart(nameStartsWith))
                .ReturnsAsync(characterSeriesDTO);

            // Act
            var result = await _sut.GetCharactersFromNamePart(nameStartsWith);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(characterSeriesDTO);
        }


        [Fact]
        public async Task GetThanosInfoPdf_WhenGenerateThanosInfoPdfReturnsData_ShouldReturnFileResult()
        {
            // Arrange
            var mockPdfData = new byte[] { 0x25, 0x50, 0x44, 0x46 };
            _marvelServiceMock.Setup(m => m.GenerateThanosInfoPdf()).ReturnsAsync(mockPdfData);

            // Act
            var result = await _sut.GetThanosInfoPdf();

            // Assert
            var fileResult = result.Should().BeOfType<FileContentResult>().Subject;
            fileResult.FileContents.Should().Equal(mockPdfData);
            fileResult.ContentType.Should().Be("application/pdf");
            fileResult.FileDownloadName.Should().Be("thanos_report.pdf");
        }
    }
}
