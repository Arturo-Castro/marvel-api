using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarvelApp.Domain.Entities
{
    public class Character
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Url { get; set; }
        public int Strength { get; set; }
        public int Intelligence { get; set; }
        public int Speed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        [ForeignKey("RescueTeam")]
        public int? RescueTeamId { get; set; }
        public RescueTeam? RescueTeam { get; set; }
    }
}
