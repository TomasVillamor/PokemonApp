namespace PokemonApp.Domain.Models;

public class Pokemon
{
    public int Id { get; set; }
    public int? PokeApiId { get; set; }
    public string Name { get; set; } = null!;
    public int Height { get; set; }
    public int Weight { get; set; }
}

