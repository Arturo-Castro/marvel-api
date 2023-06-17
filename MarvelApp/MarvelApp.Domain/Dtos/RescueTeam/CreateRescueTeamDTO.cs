using System.ComponentModel.DataAnnotations;

namespace MarvelApp.Domain.Dtos.RescueTeam
{
    public class CreateRescueTeamDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; } = null!;
        [Range(1, int.MaxValue)]
        public int CharacterId { get; set; }
    }
}
