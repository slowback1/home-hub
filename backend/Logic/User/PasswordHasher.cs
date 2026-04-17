using System;
using System.Security.Cryptography;
using Common.Interfaces;

namespace Logic.User;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password cannot be null or empty");

        var salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        byte[] hash;
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithm))
        {
            hash = pbkdf2.GetBytes(KeySize);
        }

        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword)) return false;

            var parts = hashedPassword.Split('.');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);

            byte[] hashToCompare;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithm))
            {
                hashToCompare = pbkdf2.GetBytes(KeySize);
            }

            // Use time-constant comparison
            return CryptographicOperations.FixedTimeEquals(hash, hashToCompare);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error verifying password:");
            Console.WriteLine(e);
            return false;
        }
    }
}