using PokemonApp.Domain.Models;
using PokemonApp.Services.Helpers;
using PokemonApp.Services.Interfaces;
using PokemonApp.DataAcess.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace PokemonApp.Services.Services;

public class AdminSeeder : ISeeder
{
    private readonly IUserRepository _userRepository;
    private readonly AdminUserSettings _settings;

    public AdminSeeder(IUserRepository userRepository, IOptions<AdminUserSettings> settings)
    {
        _userRepository = userRepository;
        _settings = settings.Value;
    }

    public async Task SeedAsync()
    {
        var existingAdmin = await _userRepository.GetByEmailAsync(_settings.Email);
        if (existingAdmin == null)
        {
            PasswordHelper.CreatePasswordHash(_settings.Password, out var hash, out var salt);

            var adminUser = new User
            {
                Username = _settings.Username,
                Email = _settings.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                RoleId = 1, //admin
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(adminUser);
        }
    }
}

