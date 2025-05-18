using PokemonApp.Domain.Models;

namespace PokemonApp.DataAcess.Repositories.Interfaces;
public interface IPokemonRepository
{
    Task<IEnumerable<Pokemon>> GetAllAsync();
    Task<Pokemon?> GetByIdAsync(int id);
    Task AddAsync(Pokemon pokemon);
    Task UpdateAsync(Pokemon pokemon);
    Task DeleteAsync(Pokemon pokemon);
    Task<bool> ExistsByNameAsync(string name);
    Task AddRangeAsync(List<Pokemon> pokemons);
    Task<List<Pokemon>> GetByPokeApiIdsAsync(IEnumerable<int> pokeApiIds);
    Task UpdateRangeAsync(IEnumerable<Pokemon> pokemons);

}
