using Microsoft.AspNetCore.Mvc;
using PokemonApp.Domain.Dtos.Auth;
using PokemonApp.Domain.Reponses;
using PokemonApp.Services.Interfaces;

namespace PokemonApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var token = await _authService.RegisterAsync(dto);
            return Ok(ApiResponse<string>.Ok(token, "Se registro correctamente"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var token = await _authService.LoginAsync(dto);
            return Ok(ApiResponse<string>.Ok(token, "Login exitoso"));
        }
        catch (Exception ex)
        {
            return Unauthorized(ApiResponse<string>.Fail(ex.Message));
        }
    }
}

