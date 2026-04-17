using Common.Interfaces;

namespace TestUtilities;

public class TestPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return $"hashed:{password}";
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return hashedPassword == HashPassword(password);
    }
}