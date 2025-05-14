using PokemonApp.DataAcess;
using PokemonApp.Domain.Models;
using PokemonApp.Services.Helpers;
using PokemonApp.Services.Interfaces;
using PokemonApp.DataAcess.Repositories.Interfaces;
namespace PokemonApp.Services.Services;
public class AdminSeeder : ISeeder
{
    private readonly IUserRepository _userRepository;

    public AdminSeeder(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task SeedAsync()
    {
        var existingAdmin = await _userRepository.GetByEmailAsync("admin@test.com");
        if (existingAdmin == null)
        {
            PasswordHelper.CreatePasswordHash("admin123", out var hash, out var salt);

            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@test.com",
                PasswordHash = hash,
                PasswordSalt = salt,
                RoleId = 1, // Admin
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(adminUser);
        }
    }
}
