using Common.Interfaces;
using Logic.User;

namespace Logic.Tests.User;

[TestFixture]
public class PasswordHasherTests
{
    [SetUp]
    public void SetUp()
    {
        _hasher = new PasswordHasher();
    }

    private IPasswordHasher _hasher;

    [Test]
    public void HashPassword_ShouldThrow_OnNullOrEmpty()
    {
        Assert.Throws<ArgumentException>(() => _hasher.HashPassword(null!));
        Assert.Throws<ArgumentException>(() => _hasher.HashPassword(""));
    }

    [Test]
    public void HashPassword_ShouldReturnDifferentHashes_ForSamePassword()
    {
        var hash1 = _hasher.HashPassword("Password123!");
        var hash2 = _hasher.HashPassword("Password123!");
        Assert.That(hash2, Is.Not.EqualTo(hash1), "Hashes with different salts should not match");
    }

    [Test]
    public void VerifyPassword_ShouldReturnTrue_ForCorrectPassword()
    {
        var password = "MySecurePassword!";
        var hash = _hasher.HashPassword(password);
        Assert.IsTrue(_hasher.VerifyPassword(password, hash));
    }

    [Test]
    public void VerifyPassword_ShouldReturnFalse_ForWrongPassword()
    {
        var hash = _hasher.HashPassword("RightPassword");
        Assert.IsFalse(_hasher.VerifyPassword("WrongPassword", hash));
    }

    [Test]
    [TestCase("password", "not-a-valid-hash")]
    [TestCase("password", "")]
    [TestCase("", "")]
    public void VerifyPassword_ShouldReturnFalse_ForMalformedHash(string password, string hash)
    {
        Assert.IsFalse(_hasher.VerifyPassword(password, hash));
    }
}