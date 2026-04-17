using Common.Models;
using Logic.Authorization;
using Logic.User;
using TestUtilities.CrudImplementations;
using TestUtilities.TestData;

namespace Logic.Tests.Authorization;

[TestFixture]
public class AuthorizationUseCaseTests
{
    [Test]
    [TestCase("X-User-Token")]
    [TestCase("x-user-token")]
    [TestCase("X-USER-TOKEN")]
    public void GetTokenFromHeaders_ReturnsToken_WhenHeaderExists(string tokenName)
    {
        var headers = new Dictionary<string, string> { { tokenName, "token123" } };
        var useCase = new UserAuthorizationUseCase(new TestCrudFactory(), TestTokenData.TestValidTokenConfig);
        var token = useCase.GetTokenFromHeaders(headers);
        Assert.That(token, Is.EqualTo("token123"));
    }

    [Test]
    public void GetTokenFromHeaders_ReturnsEmpty_WhenHeaderMissing()
    {
        var headers = new Dictionary<string, string>();
        var useCase = new UserAuthorizationUseCase(new TestCrudFactory(),
            TestTokenData.TestValidTokenConfig);
        var token = useCase.GetTokenFromHeaders(headers);
        Assert.That(token, Is.EqualTo(string.Empty));
    }

    [Test]
    public async Task AuthorizeAsync_ReturnsSuccess_WhenTokenValidAndUserExists()
    {
        var user = new AppUser { Name = "TestUser" };
        var tokenConfig = TestTokenData.TestValidTokenConfig;
        var tokenHandler = new TokenHandler(tokenConfig);
        var token = tokenHandler.GenerateToken(user);
        var crudFactory = new TestCrudFactory();
        await crudFactory.GetCrud<AppUser>().CreateAsync(user);
        var useCase = new UserAuthorizationUseCase(crudFactory, tokenConfig);
        var result = await useCase.AuthorizeAsync(token);
        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result.Status, Is.EqualTo(AuthorizationStatus.Authenticated));
        Assert.That(result.Result.User, Is.Not.Null);
        Assert.That(result.Result.User.Name, Is.EqualTo("TestUser"));
        Assert.That(result.Result.Token, Is.EqualTo(token));
    }

    [Test]
    public async Task AuthorizeAsync_ReturnsFailure_WhenTokenInvalid()
    {
        var tokenConfig = TestTokenData.TestValidTokenConfig;
        var crudFactory = new TestCrudFactory();
        var useCase = new UserAuthorizationUseCase(crudFactory, tokenConfig);
        var result = await useCase.AuthorizeAsync("invalidtoken");
        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result.Status, Is.EqualTo(AuthorizationStatus.InvalidCredentials));
        Assert.That(result.Result.ErrorMessage, Is.Not.Null);
    }

    [Test]
    public async Task AuthorizeAsync_ReturnsFailure_WhenUserNotFound()
    {
        var user = new AppUser { Id = "user2", Name = "TestUser2" };
        var tokenConfig = TestTokenData.TestValidTokenConfig;
        var tokenHandler = new TokenHandler(tokenConfig);
        var token = tokenHandler.GenerateToken(user);
        var crudFactory = new TestCrudFactory();
        var useCase = new UserAuthorizationUseCase(crudFactory, tokenConfig);
        var result = await useCase.AuthorizeAsync(token);
        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result.Status, Is.EqualTo(AuthorizationStatus.UserNotFound));
        Assert.That(result.Result.ErrorMessage, Is.EqualTo("User not found."));
    }
}