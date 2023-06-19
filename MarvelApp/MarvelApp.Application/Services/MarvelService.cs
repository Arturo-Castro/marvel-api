using MarvelApp.Application.Interfaces;
using MarvelApp.Application.Interfaces.ApiRestServices;
using MarvelApp.Domain.Dtos;
using MarvelApp.Domain.Dtos.MarvelApiRestService;
using Microsoft.Extensions.Configuration;
using IronPdf;
using Refit;
using System.Security.Cryptography;
using System.Text;
using static MarvelApp.Domain.Dtos.MarvelApiRestService.MarvelResponseDTO;

namespace MarvelApp.Application.Services
{
    public class MarvelService : IMarvelService
    {
        private readonly string urlMarvelApi;
        private readonly IMarvelApiRestService _marvelApiRestService;
        private readonly string apiKey;
        private readonly string privateKey;

        public MarvelService(IConfiguration configuration, IMarvelApiRestService marvelApiRestService)
        {
            urlMarvelApi = configuration.GetSection("Urls:MarvelApi").Value ?? throw new Exception("Marvel api URL does not exist");
            _marvelApiRestService = marvelApiRestService;
            //_marvelApiRestService = RestService.For<IMarvelApiRestService>(urlMarvelApi);
            apiKey = Environment.GetEnvironmentVariable("PUBLIC_KEY") ?? throw new Exception("The public key does not exist");
            privateKey = Environment.GetEnvironmentVariable("PRIVATE_KEY") ?? throw new Exception("The private key does not exist");
        }

        public async Task<List<CharacterSeriesDTO>> GetCharactersFromNamePart(string nameStartsWith)
        {            
            string ts = DateTime.Now.Ticks.ToString();
            string hash = GetMd5Hash(ts + privateKey + apiKey);
            var marvelApiResponse = await _marvelApiRestService.GetAllCharactersAsync(nameStartsWith, apiKey, hash, ts);
            if(marvelApiResponse.Data.Results.Count == 0)
            {
                return marvelApiResponse.Data.Results;
            }
            return FilterCharactersByAvengersSeries(marvelApiResponse.Data.Results);                                     
        }

        public async Task<byte[]> GenerateThanosInfoPdf()
        {
            string ts = DateTime.Now.Ticks.ToString();
            string hash = GetMd5Hash(ts + privateKey + apiKey);
            var thanosCharacterResponse = await _marvelApiRestService.GetThanosCharacter(apiKey, hash, ts);
            var thanosComicsResponse = await _marvelApiRestService.GetThanosCharacterStories(apiKey, hash, ts);
            List<ThanosComicsItemDTO> thanosLastFiveComics = thanosComicsResponse.Data.Results[0].Comics.Items.TakeLast(5).ToList();
            string htmlContent = GenerateHtmlContent(thanosCharacterResponse.Data.Results[0], thanosLastFiveComics);

            var renderer = new HtmlToPdf();
            var pdf = await renderer.RenderHtmlAsPdfAsync(htmlContent);

            return pdf.BinaryData;
        }


        #region Private methods

        private string GetMd5Hash(string input)
        {
            using(MD5 md5 = MD5.Create())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sbuilder = new StringBuilder();
                for(int i = 0; i < data.Length; i++)
                {
                    sbuilder.Append(data[i].ToString("x2"));
                }
                return sbuilder.ToString();
            }
        }

        private List<CharacterSeriesDTO> FilterCharactersByAvengersSeries(List<CharacterSeriesDTO> characters)
        {
            return characters.Where(x => x.Series.Items.Any(s => s.Name.Contains("Avengers"))).ToList();
        }

        private static string GenerateHtmlContent(ThanosCharacterDTO thanosCharacter, List<ThanosComicsItemDTO> thanosLastFiveComics)
        {
            string characterName = thanosCharacter.Name;
            string characterDescription = thanosCharacter.Description;
            string characterThumbnailUrl = thanosCharacter.Thumbnail.GetImageUrl();

            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.AppendLine("<html>");
            htmlBuilder.AppendLine("<head>");
            htmlBuilder.AppendLine("<title>Thanos Character Information</title>");
            htmlBuilder.AppendLine("</head>");
            htmlBuilder.AppendLine("<body>");
            htmlBuilder.AppendLine("<h1>Thanos Character Information</h1>");
            htmlBuilder.AppendLine("<div>");
            htmlBuilder.AppendLine($"<img src=\"{characterThumbnailUrl}\" alt=\"{characterName} Thumbnail\"/>");
            htmlBuilder.AppendLine($"<h2>{characterName}</h2>");
            htmlBuilder.AppendLine($"<p>{characterDescription}</p>");
            htmlBuilder.AppendLine("</div>");
            htmlBuilder.AppendLine("<h2>Comics</h2>");
            htmlBuilder.AppendLine("<ul>");

            foreach (var comic in thanosLastFiveComics)
            {
                string comicResourceURI = comic.Name;
                htmlBuilder.AppendLine($"<li>{comicResourceURI}</li>");
            }

            htmlBuilder.AppendLine("</ul>");

            htmlBuilder.AppendLine("</body>");
            htmlBuilder.AppendLine("</html>");
            return htmlBuilder.ToString();
        }

        #endregion
    }
}
