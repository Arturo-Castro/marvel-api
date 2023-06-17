using MarvelApp.Domain.Dtos.MarvelApiRestService;
using Refit;
using static MarvelApp.Domain.Dtos.MarvelApiRestService.MarvelResponseDTO;

namespace MarvelApp.Application.Interfaces.ApiRestServices
{
    public interface IMarvelApiRestService
    {
        [Get("/v1/public/characters")]
        Task<ApiResponseDTO> GetAllCharactersAsync
            (
                string nameStartsWith, 
                [AliasAs("apikey")] string apiKey, 
                string hash, 
                string ts
            );
    }
}
