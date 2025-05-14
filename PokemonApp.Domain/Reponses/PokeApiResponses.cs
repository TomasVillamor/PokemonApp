namespace PokemonApp.Domain.Reponses;
public class PokeApiResponses
{
    public class PokeApiResponseDto
    {
        public int Count { get; set; }
        public string? Next { get; set; }
        public List<PokeApiResultDto> Results { get; set; } = new();
    }

    public class PokeApiResultDto
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public class PokeApiPokemonDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Height { get; set; }
        public int Weight { get; set; }
    }
}
