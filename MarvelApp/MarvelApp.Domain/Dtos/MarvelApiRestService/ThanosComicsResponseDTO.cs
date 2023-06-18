namespace MarvelApp.Domain.Dtos.MarvelApiRestService
{
    public class ThanosComicsResponseDTO
    {
        public int Code { get; set; }
        public string Status { get; set; }
        public ThanosComicsDataDTO Data { get; set; }
    }

    public class ThanosComicsDataDTO
    {
        public List<ThanosComicsResultDTO> Results { get; set; }
    }

    public class ThanosComicsResultDTO
    {
        public ThanosComicsItemsDTO Comics { get; set; }
        public string ResourceURI { get; set; }
    }

    public class ThanosComicsItemsDTO
    {
        public List<ThanosComicsItemDTO> Items { get; set; }
    }

    public class ThanosComicsItemDTO
    {
        public string Name { get; set; }
        public string ResourceURI { get; set; }
    }
}
