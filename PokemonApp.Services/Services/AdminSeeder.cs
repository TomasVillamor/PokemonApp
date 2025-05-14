using PokemonApp.DataAcess;
using PokemonApp.Domain.Models;
using PokemonApp.Services.Helpers;
using PokemonApp.Services.Interfaces;

namespace PokemonApp.Services.Services;
public class AdminSeeder : ISeeder
{
    private readonly ApplicationDbContext _context;

    public AdminSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (!_context.Users.Any(u => u.Email == "admin@test.com"))
        {
            PasswordHelper.CreatePasswordHash("admin123", out var hash, out var salt);

            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@test.com",
                PasswordHash = hash,
                PasswordSalt = salt,
                RoleId = 1, // Rol "Admin"
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();
        }
    }
}
