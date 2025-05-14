using PokemonApp.DataAcess.Repositories.Interfaces;
using PokemonApp.Domain.Dtos.Auth;
using PokemonApp.Domain.Models;
using PokemonApp.Services.Helpers;
using PokemonApp.Services.Interfaces;

namespace PokemonApp.Services.Services;
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            throw new Exception("El email ya está registrado.");
        }

        PasswordHelper.CreatePasswordHash(dto.Password, out var hash, out var salt);

        User user = new()
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            RoleId = 2, // Rol "User"
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        var savedUser = await _userRepository.GetByEmailAsync(user.Email);

        if (savedUser == null)
        {
            throw new Exception("Error interno al registrar el usuario.");
        }

        return _jwtService.GenerateToken(savedUser);
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);

        if (user == null)
        {
            throw new Exception("No existe el usuario con el mail ingresado");

        }
        if (!PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt))
        {
            throw new Exception("La contraseña es incorrecta");

        }
        return _jwtService.GenerateToken(user);
    }

}
