using AutoMapper;
using PokemonApp.DataAcess.Repositories.Interfaces;
using PokemonApp.Domain.Dtos.Category;
using PokemonApp.Domain.Models;
using PokemonApp.Services.Interfaces;

namespace PokemonApp.Services.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryReadDto>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryReadDto>>(categories);
    }

    public async Task<CategoryReadDto?> GetByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category == null ? null : _mapper.Map<CategoryReadDto>(category);
    }

    public async Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto)
    {
        // Check if a category with the same name already exists
        if (await _categoryRepository.ExistsByNameAsync(dto.Name))
        {
            throw new InvalidOperationException("Ya existe una categoría con ese nombre.");
        }

        var category = _mapper.Map<Category>(dto);
        var created = await _categoryRepository.CreateAsync(category);
        return _mapper.Map<CategoryReadDto>(created);
    }

    public async Task<CategoryReadDto> UpdateAsync(int id, CategoryUpdateDto dto)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(id);
        if (existingCategory == null)
        {
            throw new InvalidOperationException("Categoría no encontrada.");
        }

        // Check if another category with the same name already exists
        var categoryWithSameName = await _categoryRepository.GetByNameAsync(dto.Name);
        if (categoryWithSameName != null && categoryWithSameName.Id != id)
        {
            throw new InvalidOperationException("Ya existe otra categoría con ese nombre.");
        }

        _mapper.Map(dto, existingCategory);
        var updated = await _categoryRepository.UpdateAsync(existingCategory);
        return _mapper.Map<CategoryReadDto>(updated);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _categoryRepository.ExistsAsync(id))
        {
            throw new InvalidOperationException("Categoría no encontrada.");
        }

        await _categoryRepository.DeleteAsync(id);
    }
}