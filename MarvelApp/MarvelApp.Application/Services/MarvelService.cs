using MarvelApp.Application.Interfaces;
using MarvelApp.Application.Interfaces.ApiRestServices;
using MarvelApp.Domain.Dtos;
using Microsoft.Extensions.Configuration;
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

        public MarvelService(IConfiguration configuration)
        {
            urlMarvelApi = configuration.GetSection("Urls:MarvelApi").Value ?? throw new Exception("Marvel api URL does not exist");
            _marvelApiRestService = RestService.For<IMarvelApiRestService>(urlMarvelApi);
            apiKey = Environment.GetEnvironmentVariable("PUBLIC_KEY") ?? throw new Exception("The public key does not exist");
            privateKey = Environment.GetEnvironmentVariable("PRIVATE_KEY") ?? throw new Exception("The private key does not exist");
        }

        public async Task<List<CharacterSeriesDTO>> GetCharactersFromNamePart(string nameStartsWith)
        {            
            string ts = DateTime.Now.Ticks.ToString();
            string hash = GetMd5Hash(ts + privateKey + apiKey);
            var marvelApiResponse = await _marvelApiRestService.GetAllCharactersAsync(nameStartsWith, apiKey, hash, ts);
            return FilterCharactersByAvengersSeries(marvelApiResponse.Data.Results);                                     
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


        #endregion
    }
}
