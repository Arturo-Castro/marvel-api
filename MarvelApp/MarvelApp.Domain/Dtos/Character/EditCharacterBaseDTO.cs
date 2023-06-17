using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelApp.Domain.Dtos.Character
{
    public class EditCharacterBaseDTO
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
