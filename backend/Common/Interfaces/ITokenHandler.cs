using Common.Models;

namespace Common.Interfaces;

public interface ITokenHandler
{
    string GenerateToken(AppUser user);
    AppUser ReadToken(string token);
}