using PokemonApp.Domain.Dtos.Pokemon;

namespace PokemonApp.Services.Interfaces;
public interface IPokemonService
{
    Task<IEnumerable<PokemonReadDto>> GetAllAsync();
    Task<PokemonReadDto?> GetByIdAsync(int id);
    Task<PokemonReadDto> CreateAsync(PokemonCreateDto dto);
    Task<PokemonReadDto> UpdateAsync(int id, PokemonUpdateDto dto);
    Task DeleteAsync(int id);
    Task<(int Added, int Updated)> SyncFromPokeApiAsync();
}
