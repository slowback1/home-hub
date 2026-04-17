namespace Logic.User;

public class TokenGeneratorConfig
{
    public string SecretKey { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
}