using PokemonApp.Domain.Models;

namespace PokemonApp.Services.Interfaces;
public interface IJwtService
{
    string GenerateToken(User user);
}
