using FluentAssertions;
using MarvelApp.Application.Interfaces;
using MarvelApp.Application.Interfaces.ApiRestServices;
using MarvelApp.Application.Services;
using MarvelApp.Domain.Dtos.MarvelApiRestService;
using Microsoft.Extensions.Configuration;
using Moq;

namespace MarvelApp.TestApi.Systems.Services
{
    public class TestMarvelService
    {
        private readonly Mock<IMarvelApiRestService> _marvelApiRestServiceMock;
        private readonly IMarvelService _sut;
        private readonly string testApiKey;
        private readonly string testPrivateKey;

        public TestMarvelService()
        {
            var testConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Urls:MarvelApi", "https://test.marvelapi.com"),
                })
                .Build();
            Environment.SetEnvironmentVariable("PUBLIC_KEY", "test_public_key");
            Environment.SetEnvironmentVariable("PRIVATE_KEY", "test_private_key");
            testApiKey = Environment.GetEnvironmentVariable("PUBLIC_KEY");
            testPrivateKey = Environment.GetEnvironmentVariable("PRIVATE_KEY");
            _marvelApiRestServiceMock = new Mock<IMarvelApiRestService>();
            _sut = new MarvelService(testConfig, _marvelApiRestServiceMock.Object);            
        }

        [Fact]
        public async Task GetCharactersFromNamePart_WhenNoResultsFromMarvelApi_ShouldReturnEmptyList()
        {
            // Arrange
            string nameStartsWith = "test";
            var apiResponse = new MarvelResponseDTO.ApiResponseDTO
            {
                Data = new MarvelResponseDTO.DataDTO
                {
                    Results = new List<MarvelResponseDTO.CharacterSeriesDTO>()
                }
            };
            _marvelApiRestServiceMock
                .Setup(s => s.GetAllCharactersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _sut.GetCharactersFromNamePart(nameStartsWith);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCharactersFromNamePart_WhenThereAreResultsFromMarvelApi_ShouldReturnFilteredResults()
        {
            // Arrange
            string nameStartsWith = "Test Character";

            var marvelApiResponse = new MarvelResponseDTO.ApiResponseDTO
            {
                Data = new MarvelResponseDTO.DataDTO
                {
                    Results = new List<MarvelResponseDTO.CharacterSeriesDTO>
                    {
                        new MarvelResponseDTO.CharacterSeriesDTO
                        {
                            Name = "Test Character",
                            Series = new MarvelResponseDTO.SeriesDTO
                            {
                                Items = new List<MarvelResponseDTO.SeriesItemDTO>
                                {
                                    new MarvelResponseDTO.SeriesItemDTO
                                    {
                                        Name = "Avengers"
                                    }
                                }
                            }
                        },
                        new MarvelResponseDTO.CharacterSeriesDTO
                        {
                            Name = "Goku",
                            Series = new MarvelResponseDTO.SeriesDTO
                            {
                                Items = new List<MarvelResponseDTO.SeriesItemDTO>
                                {
                                    new MarvelResponseDTO.SeriesItemDTO
                                    {
                                        Name = "Dragon ball Z"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            _marvelApiRestServiceMock.Setup(_ => _.GetAllCharactersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(marvelApiResponse);

            // Act
            var result = await _sut.GetCharactersFromNamePart(nameStartsWith);

            // Assert
            result.Should().NotBeNullOrEmpty(); 
            result.Should().ContainSingle(); 
            result.First().Name.Should().Be("Test Character");
        }
    }
}
