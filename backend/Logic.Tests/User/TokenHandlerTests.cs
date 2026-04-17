using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Common.Models;
using Logic.User;
using Microsoft.IdentityModel.Tokens;
using TokenHandler = Logic.User.TokenHandler;

namespace Logic.Tests.User;

[TestFixture]
public class TokenHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        _config = new TokenGeneratorConfig { SecretKey = SecretKey, ExpiryMinutes = 60 };
        _handler = new TokenHandler(_config);
    }

    private TokenHandler _handler;
    private TokenGeneratorConfig _config;
    private const string SecretKey = "super_secret_key_for_testing_purposes_only_1234567890";

    [Test]
    public void GenerateToken_ShouldReturnValidJwt_WithUserData()
    {
        var user = new AppUser { Id = "42", Name = "testuser" };
        var token = _handler.GenerateToken(user);
        Assert.That(token, Is.Not.Null.And.Not.Empty);

        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key
        };
        handler.ValidateToken(token, parameters, out var validatedToken);
        var jwt = (JwtSecurityToken)validatedToken;
        var sub = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        var uname = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;
        Assert.That(sub, Is.EqualTo("42"));
        Assert.That(uname, Is.EqualTo("testuser"));
    }

    [Test]
    public void GenerateToken_ShouldExpireInFuture()
    {
        var user = new AppUser { Id = "1", Name = "expuser" };
        var token = _handler.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        Assert.That(jwt.ValidTo, Is.GreaterThan(DateTime.UtcNow));
    }

    [Test]
    public void ReadToken_ShouldReturnUserWithCorrectIdAndName()
    {
        var user = new AppUser { Id = "abc123", Name = "readuser" };
        var token = _handler.GenerateToken(user);
        var parsed = _handler.ReadToken(token);
        Assert.That(parsed.Id, Is.EqualTo("abc123"));
        Assert.That(parsed.Name, Is.EqualTo("readuser"));
    }

    [Test]
    public void ReadToken_ShouldThrow_OnInvalidToken()
    {
        Assert.Throws<TokenInvalidException>(() => _handler.ReadToken("not-a-jwt"));
    }

    [Test]
    public void ReadToken_ShouldThrow_OnExpiredToken()
    {
        var expiredConfig = new TokenGeneratorConfig { SecretKey = SecretKey, ExpiryMinutes = -1 };
        var expiredHandler = new TokenHandler(expiredConfig);
        var user = new AppUser { Id = "expired", Name = "expireduser" };
        var token = expiredHandler.GenerateToken(user);
        Thread.Sleep(100);
        Assert.Throws<TokenExpiredException>(() => _handler.ReadToken(token));
    }
}