using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokemonApp.Domain.Dtos.Pokemon;
using PokemonApp.Domain.Reponses;
using PokemonApp.Services.Interfaces;

namespace PokemonApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PokemonController : ControllerBase
{
    private readonly IPokemonService _pokemonService;

    public PokemonController(IPokemonService pokemonService)
    {
        _pokemonService = pokemonService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PokemonReadDto>>> GetAll()
    {
        try
        {
            var pokemons = await _pokemonService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<PokemonReadDto>>.Ok(pokemons));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail("Error al obtener los Pokémon: " + ex.Message));
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<PokemonReadDto>> GetById(int id)
    {
        try
        {
            var pokemon = await _pokemonService.GetByIdAsync(id);
            if (pokemon == null)
            {
                return NotFound(ApiResponse<string>.Fail("Pokémon no encontrado"));
            }

            return Ok(ApiResponse<PokemonReadDto>.Ok(pokemon));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail("Error al obtener el Pokémon: " + ex.Message));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PokemonReadDto>> Create([FromBody] PokemonCreateDto dto)
    {
        try
        {
            var created = await _pokemonService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<PokemonReadDto>.Ok(created, "Pokémon creado correctamente"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail("Error al crear el Pokémon: " + ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] PokemonUpdateDto dto)
    {
        try
        {
            var updated = await _pokemonService.UpdateAsync(id, dto);
            return Ok(ApiResponse<PokemonReadDto>.Ok(updated, "Pokémon actualizado correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _pokemonService.DeleteAsync(id);
            return Ok(ApiResponse<string>.Ok("Pokémon eliminado correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }



    [HttpPost("sync")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Sync()
    {
        try
        {
            var count = await _pokemonService.SyncFromPokeApiAsync();
            return Ok(ApiResponse<string>.Ok($"Se sincronizaron {count} Pokémon desde la PokéAPI."));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail("Error en la sincronización: " + ex.Message));
        }
    }

}