using static MarvelApp.Domain.Dtos.MarvelApiRestService.MarvelResponseDTO;

namespace MarvelApp.Application.Interfaces
{
    public interface IMarvelService
    {
        Task<List<CharacterSeriesDTO>> GetCharactersFromNamePart(string nameStartsWith);
        Task<byte[]>GenerateThanosInfoPdf();
    }
}
