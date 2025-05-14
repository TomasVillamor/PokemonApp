using AutoMapper;
using PokemonApp.Domain.Dtos.Pokemon;
using PokemonApp.Domain.Models;

namespace PokemonApp.Domain.Mapping;
public class PokemonProfile : Profile
{
    public PokemonProfile()
    {
        CreateMap<Pokemon, PokemonReadDto>();
        CreateMap<PokemonCreateDto, Pokemon>();
        CreateMap<PokemonUpdateDto, Pokemon>();
    }
}
