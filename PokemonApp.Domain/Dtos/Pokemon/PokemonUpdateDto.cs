using System.ComponentModel.DataAnnotations;

namespace PokemonApp.Domain.Dtos.Pokemon;

public class PokemonUpdateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Height { get; set; }

    [Range(1, int.MaxValue)]
    public int Weight { get; set; }
}
