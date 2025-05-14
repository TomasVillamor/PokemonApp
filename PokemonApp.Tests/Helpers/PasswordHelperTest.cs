using PokemonApp.Services.Helpers;

namespace PokemonApp.Tests.Helpers;
public class PasswordHelperTest
{
    [Fact]
    public void CreatePasswordHash_ShouldGenerateHashAndSalt()
    {
        var password = "contraseña123";

        PasswordHelper.CreatePasswordHash(password, out var hash, out var salt);

        Assert.NotNull(hash);
        Assert.NotNull(salt);
        Assert.NotEmpty(hash);
        Assert.NotEmpty(salt);
        Assert.True(hash.Length > 0);
        Assert.True(salt.Length > 0);
    }

    [Fact]
    public void CreatePasswordHash_ShouldDifferentHash()
    {
        var password = "mismacontraseña";

        PasswordHelper.CreatePasswordHash(password, out var hash1, out var salt1);
        PasswordHelper.CreatePasswordHash(password, out var hash2, out var salt2);

        Assert.False(hash1.SequenceEqual(hash2));
        Assert.False(salt1.SequenceEqual(salt2));
    }

    [Fact]
    public void VerifyPassword_PasswordCorrect_ShouldReturnTrue()
    {
        var password = "contraseña123";
        PasswordHelper.CreatePasswordHash(password, out var hash, out var salt);

        var result = PasswordHelper.VerifyPassword(password, hash, salt);

        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_PasswordIncorrect_ShouldReturnFalse()
    {
        var originalPassword = "contraseña123";
        var wrongPassword = "otracontraseña";
        PasswordHelper.CreatePasswordHash(originalPassword, out var hash, out var salt);

        var result = PasswordHelper.VerifyPassword(wrongPassword, hash, salt);

        Assert.False(result);
    }


}
