using PokemonApp.DataAcess.Repositories.Interfaces;
using PokemonApp.Domain.Dtos.Auth;
using PokemonApp.Domain.Models;
using PokemonApp.Services.Helpers;
using PokemonApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace PokemonApp.Services.Services;
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;
    public AuthService(IUserRepository userRepository, IJwtService jwtService, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        _logger.LogInformation("Intentando registrar usuario: {Email}", dto.Email);
        var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            _logger.LogWarning("Registro fallido: el email {Email} ya está en uso", dto.Email);
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
            _logger.LogError("Registro fallido: el usuario {Email} no fue guardado correctamente", user.Email);
            throw new Exception("Error interno al registrar el usuario.");
        }

        _logger.LogInformation("Usuario registrado exitosamente: {Email}", user.Email);
        return _jwtService.GenerateToken(savedUser);
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        _logger.LogInformation("Intentando login para: {Email}", dto.Email);
        var user = await _userRepository.GetByEmailAsync(dto.Email);

        if (user == null)
        {
            _logger.LogWarning("Login fallido: usuario {Email} no encontrado", dto.Email);
            throw new Exception("No existe el usuario con el mail ingresado");

        }
        if (!PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt))
        {
            _logger.LogWarning("Login fallido: contraseña incorrecta para {Email}", dto.Email);
            throw new Exception("La contraseña es incorrecta");

        }
        _logger.LogInformation("Login exitoso para: {Email}", dto.Email);
        return _jwtService.GenerateToken(user);
    }

}
