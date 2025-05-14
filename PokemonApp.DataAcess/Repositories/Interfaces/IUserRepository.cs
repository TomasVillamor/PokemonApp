using PokemonApp.Domain.Models;

namespace PokemonApp.DataAcess.Repositories.Interfaces;
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
}
