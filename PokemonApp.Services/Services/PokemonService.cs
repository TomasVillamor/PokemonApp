using AutoMapper;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using PokemonApp.DataAcess.Repositories.Interfaces;
using PokemonApp.Domain.Dtos.Pokemon;
using PokemonApp.Domain.Models;
using PokemonApp.Services.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;
using static PokemonApp.Domain.Reponses.PokeApiResponses;


namespace PokemonApp.Services.Services;
public class PokemonService : IPokemonService
{

    private readonly IPokemonRepository _repository;
    private readonly IMapper _mapper;
    private readonly IHttpClientFactory _httpClientFactory;

    public PokemonService(IPokemonRepository repository, IMapper mapper, IHttpClientFactory httpClientFactory)
    {
        _repository = repository;
        _mapper = mapper;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IEnumerable<PokemonReadDto>> GetAllAsync()
    {
        var pokemons = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<PokemonReadDto>>(pokemons);
    }

    public async Task<PokemonReadDto?> GetByIdAsync(int id)
    {
        var pokemon = await _repository.GetByIdAsync(id);
        return pokemon == null ? null : _mapper.Map<PokemonReadDto>(pokemon);
    }

    public async Task<PokemonReadDto> CreateAsync(PokemonCreateDto dto)
    {

        if (await _repository.ExistsByNameAsync(dto.Name))
        {
            throw new Exception("Ya existe un Pokémon con ese nombre.");
        }

        var pokemon = _mapper.Map<Pokemon>(dto);
        pokemon.PokeApiId = null;

        await _repository.AddAsync(pokemon);

        return _mapper.Map<PokemonReadDto>(pokemon);
    }

    public async Task<PokemonReadDto> UpdateAsync(int id, PokemonUpdateDto dto)
    {
        var pokemon = await _repository.GetByIdAsync(id);
        if (pokemon == null)
        {
            throw new Exception("Pokémon no encontrado.");
        }

        if (pokemon.PokeApiId != null)
        {
            throw new Exception("No se puede modificar un Pokémon sincronizado desde la PokéAPI.");
        }

        bool nameExist = await _repository.ExistsByNameAsync(dto.Name);
        if (nameExist)
        {
            throw new Exception("Ya existe un registro con este nombre de pokemon");
        }
        pokemon.Name = dto.Name;
        pokemon.Height = dto.Height;
        pokemon.Weight = dto.Weight;

        await _repository.UpdateAsync(pokemon);
        return _mapper.Map<PokemonReadDto>(pokemon);
    }

    public async Task DeleteAsync(int id)
    {
        var pokemon = await _repository.GetByIdAsync(id);
        if (pokemon == null)
        {
            throw new Exception("Pokémon no encontrado.");
        }
        await _repository.DeleteAsync(pokemon);
    }


    public async Task<int> SyncFromPokeApiAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("PokeApi");
        int savedCount = 0;
        string? nextUrl = "pokemon?limit=100";

        while (!string.IsNullOrEmpty(nextUrl))
        {
            PokeApiResponseDto? response = null;
            try
            {
                var pageResp = await httpClient.GetAsync(nextUrl);

                if (!pageResp.IsSuccessStatusCode)
                {
                    //log
                    break;
                }

                response = await pageResp.Content.ReadFromJsonAsync<PokeApiResponseDto>();
            }
            catch
            {
                //log
                break;
            }

            if (response == null) break;

            nextUrl = response.Next;

            var detailTasks = response.Results.Select(async item =>
            {
                try
                {
                    var resp = await httpClient.GetAsync(item.Url);

                    if (!resp.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    var detail = await resp.Content.ReadFromJsonAsync<PokeApiPokemonDetailDto>();
                    return detail;
                }
                catch
                {
                    //log
                    return null;
                }
            });

            var details = await Task.WhenAll(detailTasks);

            var validDetails = details
                .Where(d => d != null)
                .Select(d => d!)
                .ToList();

            if (!validDetails.Any())
            {
                continue;
            }

            var names = validDetails.Select(d => d.Name).ToList();

            var existingNames = await _repository.GetExistingNamesAsync(names);

            var newPokemons = validDetails
                .Where(d => !existingNames.Contains(d.Name))
                .Select(d => new Pokemon
                {
                    PokeApiId = d.Id,
                    Name = d.Name,
                    Height = d.Height,
                    Weight = d.Weight
                }).ToList();

            if (newPokemons.Any())
            {
                await _repository.AddRangeAsync(newPokemons);
                savedCount += newPokemons.Count;
            }

        }

        return savedCount;
    }



}
