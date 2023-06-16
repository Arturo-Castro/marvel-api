using MarvelApp.Domain.Dtos.RescueTeam;

namespace MarvelApp.Domain.Dtos.Character
{
    public class CharacterDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Url { get; set; }
        public int Strength { get; set; }
        public int Intelligence { get; set; }
        public int Speed { get; set; }
        public RescueTeamBaseDTO? RescueTeam { get; set; }
    }
}
