using Moq;
using AutoMapper;
using PokemonApp.Domain.Models;
using PokemonApp.DataAcess.Repositories.Interfaces;
using PokemonApp.Domain.Dtos.Pokemon;
using PokemonApp.Services.Services;
using Microsoft.Extensions.Logging;
namespace PokemonApp.Tests.Services;

public class PokemonServiceTest
{
    [Fact]
    public async Task GetAllAsync_ReturnsMappedPokemonDtos()
    {
        var fakePokemons = new List<Pokemon>
        {
            new(){Id = 1, PokeApiId = 1, Name = "bulbasaur", Height = 7, Weight = 69},
            new(){Id = 2, PokeApiId = 4, Name = "charmander", Height = 6, Weight = 85}
        };
        var expectedDtos = new List<PokemonReadDto>
        {
            new(){Id = 1, Name = "bulbasaur", Height = 7, Weight = 69 },
            new(){Id = 2, Name = "charmander", Height = 6, Weight = 85}
        };

        var repoMock = new Mock<IPokemonRepository>();
        repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(fakePokemons);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map<IEnumerable<PokemonReadDto>>(fakePokemons))
                  .Returns(expectedDtos);

        var service = new PokemonService(repoMock.Object, mapperMock.Object, Mock.Of<IHttpClientFactory>(), Mock.Of<ILogger<PokemonService>>());

        var result = await service.GetAllAsync();

        Assert.Equal(expectedDtos, result);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsMappedPokemonDto()
    {
        var pokemon = new Pokemon { Id = 1, PokeApiId = 1, Name = "bulbasaur", Height = 7, Weight = 69 };
        var expectedDto = new PokemonReadDto { Id = 1, Name = "bulbasaur", Height = 7, Weight = 69 };

        var repoMock = new Mock<IPokemonRepository>();
        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pokemon);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map<PokemonReadDto>(pokemon)).Returns(expectedDto);

        var service = new PokemonService(repoMock.Object, mapperMock.Object, Mock.Of<IHttpClientFactory>(), Mock.Of<ILogger<PokemonService>>());

        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(expectedDto.Id, result!.Id);
        Assert.Equal(expectedDto.Name, result.Name);
        Assert.Equal(expectedDto.Height, result.Height);
        Assert.Equal(expectedDto.Weight, result.Weight);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        var repoMock = new Mock<IPokemonRepository>();
        repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Pokemon?)null);

        var service = new PokemonService(repoMock.Object, Mock.Of<IMapper>(), Mock.Of<IHttpClientFactory>(), Mock.Of<ILogger<PokemonService>>());

        var result = await service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_NewPokemon_ReturnsMappedPokemonReadDto()
    {
        var createDto = new PokemonCreateDto { Name = "pikachu", Height = 4, Weight = 60 };
        var entity = new Pokemon { Id = 1, Name = "pikachu", PokeApiId = null, Height = 4, Weight = 60 };
        var expectedDto = new PokemonReadDto { Id = 1, Name = "pikachu", Height = 4, Weight = 60 };

        var repoMock = new Mock<IPokemonRepository>();
        repoMock.Setup(r => r.ExistsByNameAsync(createDto.Name)).ReturnsAsync(false);
        repoMock.Setup(r => r.AddAsync(It.IsAny<Pokemon>())).Returns(Task.CompletedTask);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map<Pokemon>(createDto)).Returns(entity);
        mapperMock.Setup(m => m.Map<PokemonReadDto>(entity)).Returns(expectedDto);

        var service = new PokemonService(repoMock.Object, mapperMock.Object, Mock.Of<IHttpClientFactory>(), Mock.Of<ILogger<PokemonService>>());

        var result = await service.CreateAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal(expectedDto.Id, result.Id);
        Assert.Equal(expectedDto.Name, result.Name);
        Assert.Equal(expectedDto.Height, result.Height);
        Assert.Equal(expectedDto.Weight, result.Weight);
    }

    [Fact]
    public async Task CreateAsync_ExistingName_ThrowsException()
    {
        var dto = new PokemonCreateDto { Name = "pikachu", Height = 4, Weight = 60 };
        var repoMock = new Mock<IPokemonRepository>();
        repoMock.Setup(r => r.ExistsByNameAsync(dto.Name)).ReturnsAsync(true);

        var service = new PokemonService(repoMock.Object, Mock.Of<IMapper>(), Mock.Of<IHttpClientFactory>(), Mock.Of<ILogger<PokemonService>>());

        var ex = await Assert.ThrowsAsync<Exception>(() => service.CreateAsync(dto));
        Assert.Equal("Ya existe un Pokémon con ese nombre.", ex.Message);

        repoMock.Verify(r => r.AddAsync(It.IsAny<Pokemon>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingId_ThrowsException()
    {
        var repoMock = new Mock<IPokemonRepository>();
        repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Pokemon?)null);

        var service = new PokemonService(repoMock.Object, Mock.Of<IMapper>(), Mock.Of<IHttpClientFactory>(), Mock.Of<ILogger<PokemonService>>());

        var dto = new PokemonUpdateDto { Name = "pikachu", Height = 4, Weight = 60 };

        var ex = await Assert.ThrowsAsync<Exception>(() => service.UpdateAsync(99, dto));
        Assert.Equal("Pokémon no encontrado.", ex.Message);

        repoMock.Verify(r => r.UpdateAsync(It.IsAny<Pokemon>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_PokeApiSyncedPokemon_ThrowsException()
    {
        var pokemon = new Pokemon
        {
            Id = 1,
            PokeApiId = 25,
            Name = "pikachu",
            Height = 4,
            Weight = 60
        };

        var repoMock = new Mock<IPokemonRepository>();
        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pokemon);

        var service = new PokemonService(repoMock.Object, Mock.Of<IMapper>(), Mock.Of<IHttpClientFactory>(), Mock.Of<ILogger<PokemonService>>());

        var dto = new PokemonUpdateDto { Name = "pikachu", Height = 5, Weight = 65 };

        var ex = await Assert.ThrowsAsync<Exception>(() => service.UpdateAsync(1, dto));
        Assert.Equal("No se puede modificar un Pokémon sincronizado desde la PokéAPI.", ex.Message);

        repoMock.Verify(r => r.UpdateAsync(It.IsAny<Pokemon>()), Times.Never);
    }
    [Fact]
    public async Task UpdateAsync_ExistingName_ThrowsException()
    {
        var pokemon = new Pokemon
        {
            Id = 1,
            PokeApiId = null,
            Name = "bulbasaur",
            Height = 7,
            Weight = 69
        };

        var dto = new PokemonUpdateDto { Name = "pikachu", Height = 5, Weight = 65 };

        var repoMock = new Mock<IPokemonRepository>();
        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pokemon);
        repoMock.Setup(r => r.ExistsByNameAsync(dto.Name)).ReturnsAsync(true);

        var service = new PokemonService(repoMock.Object, Mock.Of<IMapper>(), Mock.Of<IHttpClientFactory>(), Mock.Of<ILogger<PokemonService>>());

        var ex = await Assert.ThrowsAsync<Exception>(() => service.UpdateAsync(1, dto));
        Assert.Equal("Ya existe un registro con este nombre de pokemon", ex.Message);

        repoMock.Verify(r => r.UpdateAsync(It.IsAny<Pokemon>()), Times.Never);
    }
    [Fact]
    public async Task UpdateAsync_ExistingIdAndNotPokeApiSynced_ReturnsMappedPokemonDto()
    {
        var pokemon = new Pokemon
        {
            Id = 1,
            PokeApiId = null,
            Name = "bulbasaur",
            Height = 7,
            Weight = 69
        };

        var dto = new PokemonUpdateDto { Name = "charizard", Height = 10, Weight = 130 };

        var updatedDto = new PokemonReadDto
        {
            Id = 1,
            Name = "charizard",
            Height = 10,
            Weight = 130
        };

        var repoMock = new Mock<IPokemonRepository>();
        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pokemon);
        repoMock.Setup(r => r.ExistsByNameAsync(dto.Name)).ReturnsAsync(false);
        repoMock.Setup(r => r.UpdateAsync(pokemon)).Returns(Task.CompletedTask);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map<PokemonReadDto>(pokemon)).Returns(updatedDto);

        var service = new PokemonService(repoMock.Object, mapperMock.Object, Mock.Of<IHttpClientFactory>(), Mock.Of<ILogger<PokemonService>>());

        var result = await service.UpdateAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal(updatedDto.Id, result.Id);
        Assert.Equal(updatedDto.Name, result.Name);
        Assert.Equal(updatedDto.Height, result.Height);
        Assert.Equal(updatedDto.Weight, result.Weight);

        repoMock.Verify(r => r.UpdateAsync(pokemon), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_DeletesPokemon()
    {
        var pokemon = new Pokemon { Id = 1 };

        var repoMock = new Mock<IPokemonRepository>();
        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pokemon);
        repoMock.Setup(r => r.DeleteAsync(pokemon)).Returns(Task.CompletedTask);

        var service = new PokemonService(repoMock.Object, Mock.Of<IMapper>(), Mock.Of<IHttpClientFactory>(), Mock.Of<ILogger<PokemonService>>());

        await service.DeleteAsync(1);

        repoMock.Verify(r => r.DeleteAsync(pokemon), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_ThrowsException()
    {
        var repoMock = new Mock<IPokemonRepository>();
        repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Pokemon?)null);

        var service = new PokemonService(repoMock.Object, Mock.Of<IMapper>(), Mock.Of<IHttpClientFactory>(), Mock.Of<ILogger<PokemonService>>());

        var ex = await Assert.ThrowsAsync<Exception>(() => service.DeleteAsync(99));
        Assert.Equal("Pokémon no encontrado.", ex.Message);

        repoMock.Verify(r => r.DeleteAsync(It.IsAny<Pokemon>()), Times.Never);
    }

}