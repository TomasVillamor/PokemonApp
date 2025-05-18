using Microsoft.EntityFrameworkCore;
using PokemonApp.DataAcess.Repositories.Interfaces;
using PokemonApp.Domain.Models;

namespace PokemonApp.DataAcess.Repositories;
public class PokemonRepository : IPokemonRepository
{
    private readonly ApplicationDbContext _context;

    public PokemonRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(List<Pokemon> pokemons)
    {
        await _context.Pokemons.AddRangeAsync(pokemons);
        await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<Pokemon>> GetAllAsync()
    {
        return await _context.Pokemons.ToListAsync();
    }

    public async Task<Pokemon?> GetByIdAsync(int id)
    {
        return await _context.Pokemons.FindAsync(id);
    }
    public async Task AddAsync(Pokemon pokemon)
    {
        _context.Pokemons.Add(pokemon);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(Pokemon pokemon)
    {
        _context.Pokemons.Update(pokemon);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(Pokemon pokemon)
    {
        _context.Pokemons.Remove(pokemon);
        await _context.SaveChangesAsync();
    }
    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Pokemons.AnyAsync(p => p.Name.ToLower() == name.ToLower());
    }
    public async Task<List<Pokemon>> GetByPokeApiIdsAsync(IEnumerable<int> pokeApiIds)
    {
        return await _context.Pokemons
            .Where(p => p.PokeApiId.HasValue && pokeApiIds.Contains(p.PokeApiId.Value))
            .ToListAsync();
    }
    public async Task UpdateRangeAsync(IEnumerable<Pokemon> pokemons)
    {
        _context.Pokemons.UpdateRange(pokemons);
        await _context.SaveChangesAsync();
    }
}
