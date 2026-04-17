namespace Common.Models;

public class AuthorizationResult
{
    public AuthorizationStatus Status { get; set; }
    public string? UserId { get; set; }
    public string? Token { get; set; }
    public AppUser? User { get; set; }
    public string? ErrorMessage { get; set; }
}

public enum AuthorizationStatus
{
    Authenticated,
    InvalidCredentials,
    UserNotFound,
    Error
}