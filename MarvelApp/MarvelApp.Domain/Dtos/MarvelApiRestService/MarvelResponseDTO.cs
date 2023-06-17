namespace MarvelApp.Domain.Dtos.MarvelApiRestService
{
    public class MarvelResponseDTO
    {
        public class ApiResponseDTO
        {
            public DataDTO Data { get; set; }
        }

        public class DataDTO
        {
            public List<CharacterSeriesDTO> Results { get; set; }
        }

        public class CharacterSeriesDTO
        {
            public string Name { get; set; }
            public SeriesDTO Series { get; set; }
        }

        public class SeriesDTO
        {
            public List<SeriesItemDTO> Items { get; set; }
        }

        public class SeriesItemDTO
        {
            public string Name { get; set; }
        }

    }
}
