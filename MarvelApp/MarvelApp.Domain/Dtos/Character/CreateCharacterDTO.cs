using System.ComponentModel.DataAnnotations;

namespace MarvelApp.Domain.Dtos.Character
{
    public class CreateCharacterDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Url { get; set; }
        [Range(0, 10)]
        public int Strength { get; set; }
        [Range(0, 10)]
        public int Intelligence { get; set; }
        [Range(0, 10)]
        public int Speed { get; set; }
    }
}
