using PokemonApp.Domain.Dtos.Category;

namespace PokemonApp.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryReadDto>> GetAllAsync();
    Task<CategoryReadDto?> GetByIdAsync(int id);
    Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto);
    Task<CategoryReadDto> UpdateAsync(int id, CategoryUpdateDto dto);
    Task DeleteAsync(int id);
}