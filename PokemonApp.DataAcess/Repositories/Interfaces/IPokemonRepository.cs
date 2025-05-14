using PokemonApp.Domain.Models;

namespace PokemonApp.DataAcess.Repositories.Interfaces;
public interface IPokemonRepository
{
    //Task SaveChangesAsync();
    Task<IEnumerable<Pokemon>> GetAllAsync();
    Task<Pokemon?> GetByIdAsync(int id);
    Task AddAsync(Pokemon pokemon);
    Task UpdateAsync(Pokemon pokemon);
    Task DeleteAsync(Pokemon pokemon);
    Task<bool> ExistsByNameAsync(string name);
    Task<HashSet<string>> GetExistingNamesAsync(List<string> names);
    Task AddRangeAsync(List<Pokemon> pokemons);
}
