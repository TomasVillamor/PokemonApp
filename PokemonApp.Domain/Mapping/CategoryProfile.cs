using AutoMapper;
using PokemonApp.Domain.Dtos.Category;
using PokemonApp.Domain.Models;

namespace PokemonApp.Domain.Mapping;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryReadDto>();
        CreateMap<CategoryCreateDto, Category>();
        CreateMap<CategoryUpdateDto, Category>();
    }
}