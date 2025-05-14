using Moq;
using PokemonApp.Domain.Models;
using PokemonApp.DataAcess.Repositories.Interfaces;
using PokemonApp.Services.Services;
using PokemonApp.Domain.Dtos.Auth;
using PokemonApp.Services.Interfaces;
using PokemonApp.Services.Helpers;
namespace PokemonApp.Tests.Services;
public class AuthServiceTest
{
    [Fact]
    public async Task RegisterAsync_ShouldRegisterUserAndReturnToken()
    {
        var dto = new RegisterDto
        {
            Email = "test@example.com",
            Username = "testuser",
            Password = "123456"
        };

        var repoMock = new Mock<IUserRepository>();
        var jwtMock = new Mock<IJwtService>();

        var fakeUser = new User
        {
            Id = 1,
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = new byte[32],
            PasswordSalt = new byte[32],
            RoleId = 2,
            CreatedAt = DateTime.UtcNow
        };

        repoMock.SetupSequence(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync((User?)null)
                .ReturnsAsync(fakeUser);

        repoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        jwtMock.Setup(j => j.GenerateToken(fakeUser)).Returns("fake-jwt-token");

        var service = new AuthService(repoMock.Object, jwtMock.Object);

        var result = await service.RegisterAsync(dto);

        Assert.Equal("fake-jwt-token", result);
    }


    [Fact]
    public async Task RegisterAsync_EmailAlredyRegister_ThrowsException()
    {
        var dto = new RegisterDto { Email = "test@example.com", Username = "testuser", Password = "123456" };
        var existingUser = new User { Email = dto.Email };

        var repoMock = new Mock<IUserRepository>();
        repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(existingUser);

        var jwtMock = new Mock<IJwtService>();

        var service = new AuthService(repoMock.Object, jwtMock.Object);

        await Assert.ThrowsAsync<Exception>(() => service.RegisterAsync(dto));
    }

    [Fact]
    public async Task LoginAsync_CrendentialsCorrect_ReturnToken()
    {
        var dto = new LoginDto { Email = "test@example.com", Password = "123456" };

        PasswordHelper.CreatePasswordHash(dto.Password, out var hash, out var salt);

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        var repoMock = new Mock<IUserRepository>();
        repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

        var jwtMock = new Mock<IJwtService>();
        jwtMock.Setup(j => j.GenerateToken(user)).Returns("fake-jwt-token");

        var service = new AuthService(repoMock.Object, jwtMock.Object);

        var result = await service.LoginAsync(dto);

        Assert.Equal("fake-jwt-token", result);
    }

    [Fact]
    public async Task LoginAsync_PasswordIncorrect_ThrowsException()
    {
        var dto = new LoginDto { Email = "test@example.com", Password = "wrongpass" };

        PasswordHelper.CreatePasswordHash("correctpass", out var hash, out var salt);

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        var repoMock = new Mock<IUserRepository>();
        repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

        var jwtMock = new Mock<IJwtService>();

        var service = new AuthService(repoMock.Object, jwtMock.Object);

        await Assert.ThrowsAsync<Exception>(() => service.LoginAsync(dto));
    }

    [Fact]
    public async Task LoginAsync_UserDontExist_ThrowsException()
    {
        var dto = new LoginDto { Email = "noexiste@example.com", Password = "123456" };

        var repoMock = new Mock<IUserRepository>();
        repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);

        var jwtMock = new Mock<IJwtService>();

        var service = new AuthService(repoMock.Object, jwtMock.Object);

        var ex = await Assert.ThrowsAsync<Exception>(() => service.LoginAsync(dto));
        Assert.Equal("No existe el usuario con el mail ingresado", ex.Message);
    }

}
