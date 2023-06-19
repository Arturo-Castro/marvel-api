namespace MarvelApp.Domain.Dtos.MarvelApiRestService
{
    public class ThanosCharacterResponseDTO
    {
        public int Code { get; set; }
        public string Status { get; set; }
        public DataDTO Data { get; set; }
    }

    public class DataDTO
    {
        public List<ThanosCharacterDTO> Results { get; set; }
    }

    public class ThanosCharacterDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ThumbnailDTO Thumbnail { get; set; }
    }

    public class ThumbnailDTO
    {
        public string Path { get; set; }
        public string Extension { get; set; }

        public string GetImageUrl()
        {
            return $"{Path}.{Extension}";
        }
    }
}
