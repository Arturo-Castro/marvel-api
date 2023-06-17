using MarvelApp.Domain.Dtos.Character;

namespace MarvelApp.Domain.Dtos.RescueTeam
{
    public class RescueTeamDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<CharacterBaseDTO> Characters { get; set; } = null!;
    }
}
