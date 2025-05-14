namespace PokemonApp.Domain.Dtos.Pokemon;

public class PokemonReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Height { get; set; }
    public int Weight { get; set; }
}
