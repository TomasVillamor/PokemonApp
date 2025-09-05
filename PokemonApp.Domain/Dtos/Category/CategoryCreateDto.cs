using System.ComponentModel.DataAnnotations;

namespace PokemonApp.Domain.Dtos.Category;

public class CategoryCreateDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
    public string Name { get; set; } = null!;
    
    [Required(ErrorMessage = "La descripción es requerida")]
    [StringLength(200, ErrorMessage = "La descripción no puede tener más de 200 caracteres")]
    public string Description { get; set; } = null!;
}