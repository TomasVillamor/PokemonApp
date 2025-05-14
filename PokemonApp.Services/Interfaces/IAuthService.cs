using PokemonApp.Domain.Dtos.Auth;

namespace PokemonApp.Services.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(LoginDto dto);
}
