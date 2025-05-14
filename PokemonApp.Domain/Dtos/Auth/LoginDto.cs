using System.ComponentModel.DataAnnotations;

namespace PokemonApp.Domain.Dtos.Auth;

public class LoginDto
{
    [Required(ErrorMessage = "El email es requerido.")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    [StringLength(100, ErrorMessage = "El email no puede tener más de 100 caracteres.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña de usuario es requerido.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public string Password { get; set; } = null!;
}
