using AutoMapper;
using Microsoft.Extensions.Logging;
using PokemonApp.DataAcess.Repositories.Interfaces;
using PokemonApp.Domain.Dtos.Pokemon;
using PokemonApp.Domain.Models;
using PokemonApp.Services.Interfaces;
using System.Net.Http.Json;
using static PokemonApp.Domain.Reponses.PokeApiResponses;


namespace PokemonApp.Services.Services;
public class PokemonService : IPokemonService
{

    private readonly IPokemonRepository _repository;
    private readonly IMapper _mapper;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PokemonService> _logger;

    public PokemonService(IPokemonRepository repository, IMapper mapper, IHttpClientFactory httpClientFactory, ILogger<PokemonService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<PokemonReadDto>> GetAllAsync()
    {
        _logger.LogInformation("Obteniendo todos los Pokémon...");
        var pokemons = await _repository.GetAllAsync();
        _logger.LogInformation("Se obtuvieron {Count} Pokémon", pokemons.Count());
        return _mapper.Map<IEnumerable<PokemonReadDto>>(pokemons);
    }

    public async Task<PokemonReadDto?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Buscando Pokémon con ID {Id}", id);
        var pokemon = await _repository.GetByIdAsync(id);

        if (pokemon == null)
        {
            _logger.LogWarning("No se encontró Pokémon con ID {Id}", id);
            return null;
        }

        return _mapper.Map<PokemonReadDto>(pokemon);
    }

    public async Task<PokemonReadDto> CreateAsync(PokemonCreateDto dto)
    {

        _logger.LogInformation("Creando nuevo Pokémon: {Name}", dto.Name);

        if (await _repository.ExistsByNameAsync(dto.Name))
        {
            _logger.LogWarning("Ya existe un Pokémon con el nombre {Name}", dto.Name);
            throw new Exception("Ya existe un Pokémon con ese nombre.");
        }

        var pokemon = _mapper.Map<Pokemon>(dto);
        pokemon.PokeApiId = null;

        await _repository.AddAsync(pokemon);

        _logger.LogInformation("Pokémon {Name} creado con éxito", pokemon.Name);

        return _mapper.Map<PokemonReadDto>(pokemon);
    }

    public async Task<PokemonReadDto> UpdateAsync(int id, PokemonUpdateDto dto)
    {
        _logger.LogInformation("Intentando actualizar Pokémon con ID {Id}", id);

        var pokemon = await _repository.GetByIdAsync(id);
        if (pokemon == null)
        {
            _logger.LogWarning("No se encontró el Pokémon con ID {Id} para actualizar", id);
            throw new Exception("Pokémon no encontrado.");
        }

        if (pokemon.PokeApiId != null)
        {
            _logger.LogWarning("No se puede modificar un Pokémon sincronizado desde la PokéAPI. ID: {Id}", id);
            throw new Exception("No se puede modificar un Pokémon sincronizado desde la PokéAPI.");
        }

        bool nameExists = await _repository.ExistsByNameAsync(dto.Name);
        if (nameExists)
        {
            _logger.LogWarning("El nombre {Name} ya existe. No se puede actualizar el Pokémon con ID {Id}", dto.Name, id);
            throw new Exception("Ya existe un registro con este nombre de pokemon");
        }

        pokemon.Name = dto.Name;
        pokemon.Height = dto.Height;
        pokemon.Weight = dto.Weight;

        await _repository.UpdateAsync(pokemon);
        _logger.LogInformation("Pokémon con ID {Id} actualizado exitosamente", id);

        return _mapper.Map<PokemonReadDto>(pokemon);
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Intentando eliminar Pokémon con ID {Id}", id);

        var pokemon = await _repository.GetByIdAsync(id);
        if (pokemon == null)
        {
            _logger.LogWarning("No se encontró el Pokémon con ID {Id} para eliminar", id);
            throw new Exception("Pokémon no encontrado.");
        }

        await _repository.DeleteAsync(pokemon);
        _logger.LogInformation("Pokémon con ID {Id} eliminado exitosamente", id);
    }

    public async Task<(int Added, int Updated)> SyncFromPokeApiAsync()
    {
        _logger.LogInformation("Iniciando sincronización con la PokéAPI...");
        var httpClient = _httpClientFactory.CreateClient("PokeApi");

        int totalAdded = 0;
        int totalUpdated = 0;
        var failedPages = new List<string>();
        string? nextUrl = "pokemon?limit=100";

        while (!string.IsNullOrEmpty(nextUrl))
        {
            var (success, details, next) = await FetchPageDetailsAsync(httpClient, nextUrl);

            if (!success || details == null)
            {
                _logger.LogWarning("Se omitió la página {Url} por fallo de respuesta", nextUrl);
                failedPages.Add(nextUrl);
                nextUrl = next;
                continue;
            }

            nextUrl = next;

            if (!details.Any())
            {
                continue;
            }

            var (newPokemons, updatedPokemons) = await BuildInsertAndUpdateListsAsync(details);

            if (newPokemons.Any())
            {
                await _repository.AddRangeAsync(newPokemons);
                totalAdded += newPokemons.Count;
                _logger.LogInformation("Agregados {Count} nuevos Pokémon", newPokemons.Count);
            }

            if (updatedPokemons.Any())
            {
                await _repository.UpdateRangeAsync(updatedPokemons);
                totalUpdated += updatedPokemons.Count;
                _logger.LogInformation("Actualizados {Count} Pokémon existentes", updatedPokemons.Count);
            }
        }

        _logger.LogInformation("Sincronización finalizada. Nuevos: {Added}, Actualizados: {Updated}", totalAdded, totalUpdated);

        if (failedPages.Any())
        {
            _logger.LogWarning("Fallaron {Count} páginas. URLs: {Pages}", failedPages.Count, string.Join(", ", failedPages));
        }

        return (totalAdded, totalUpdated);
    }


    private async Task<(bool Success, List<PokeApiPokemonDetailDto>? Details, string? Next)> FetchPageDetailsAsync(HttpClient httpClient, string nextUrl)
    {
        try
        {
            var pageResp = await httpClient.GetAsync(nextUrl);

            if (!pageResp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Error al obtener página {Url}. Código: {Code}", nextUrl, pageResp.StatusCode);
                return (false, null, null);
            }

            var response = await pageResp.Content.ReadFromJsonAsync<PokeApiResponseDto>();

            if (response == null)
            {
                _logger.LogWarning("La respuesta fue nula desde {Url}", nextUrl);
                return (false, null, null);
            }

            var detailTasks = response.Results.Select(async item =>
            {
                try
                {
                    var resp = await httpClient.GetAsync(item.Url);

                    if (!resp.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("Error al obtener detalle de {Name} desde {Url}", item.Name, item.Url);
                        return null;
                    }

                    return await resp.Content.ReadFromJsonAsync<PokeApiPokemonDetailDto>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Excepción al obtener detalle del Pokémon desde {Url}", item.Url);
                    return null;
                }
            });

            var details = await Task.WhenAll(detailTasks);
            var validDetails = details.Where(d => d != null).Select(d => d!).ToList();

            return (true, validDetails, response.Next);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción general al procesar la página {Url}", nextUrl);
            return (false, null, null);
        }
    }

    private async Task<(List<Pokemon> New, List<Pokemon> Updated)> BuildInsertAndUpdateListsAsync(List<PokeApiPokemonDetailDto> details)
    {
        var pokeApiIds = details.Select(d => d.Id).ToList();
        var existing = await _repository.GetByPokeApiIdsAsync(pokeApiIds);

        var newList = new List<Pokemon>();
        var updateList = new List<Pokemon>();

        foreach (var d in details)
        {
            var existingPokemon = existing.FirstOrDefault(p => p.PokeApiId == d.Id);
            if (existingPokemon == null)
            {
                newList.Add(new Pokemon
                {
                    PokeApiId = d.Id,
                    Name = d.Name,
                    Height = d.Height,
                    Weight = d.Weight
                });
            }
            else if (
                existingPokemon.Name != d.Name ||
                existingPokemon.Height != d.Height ||
                existingPokemon.Weight != d.Weight
            )
            {
                existingPokemon.Name = d.Name;
                existingPokemon.Height = d.Height;
                existingPokemon.Weight = d.Weight;

                updateList.Add(existingPokemon);
            }
        }
        return (newList, updateList);
    }

}
