using System.ComponentModel.DataAnnotations;

namespace PokemonApp.Domain.Dtos.Pokemon;

public class PokemonCreateDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
    public string Name { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "La altura debe ser mayor que 0.")]
    public int Height { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La altura debe ser mayor que 0.")]
    public int Weight { get; set; }
}
