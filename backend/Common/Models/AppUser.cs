using Common.Interfaces;
using Newtonsoft.Json;

namespace Common.Models;

public class AppUser : IIdentifyable
{
    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public string? Password { get; set; }

    public string Id { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserResponse : AppUser
{
    public string Token { get; set; } = string.Empty;
}