using Common.Interfaces;
using Common.Models;

namespace TestUtilities;

public class TestTokenHandler : ITokenHandler
{
    public string GenerateToken(AppUser user)
    {
        return $"token-for-{user.Name}";
    }

    public AppUser ReadToken(string token)
    {
        return new AppUser
        {
            Id = "42",
            Name = "TestUser"
        };
    }
}