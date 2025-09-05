using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokemonApp.Domain.Dtos.Category;
using PokemonApp.Domain.Reponses;
using PokemonApp.Services.Interfaces;

namespace PokemonApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryReadDto>>> GetAll()
    {
        try
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<CategoryReadDto>>.Ok(categories));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail("Error al obtener las categorías: " + ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryReadDto>> GetById(int id)
    {
        try
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound(ApiResponse<string>.Fail("Categoría no encontrada"));
            }

            return Ok(ApiResponse<CategoryReadDto>.Ok(category));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail("Error al obtener la categoría: " + ex.Message));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryReadDto>> Create([FromBody] CategoryCreateDto dto)
    {
        try
        {
            var created = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<CategoryReadDto>.Ok(created, "Categoría creada correctamente"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail("Error al crear la categoría: " + ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto dto)
    {
        try
        {
            var updated = await _categoryService.UpdateAsync(id, dto);
            return Ok(ApiResponse<CategoryReadDto>.Ok(updated, "Categoría actualizada correctamente."));
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
            await _categoryService.DeleteAsync(id);
            return Ok(ApiResponse<string>.Ok("Categoría eliminada correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }
}