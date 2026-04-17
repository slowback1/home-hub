using Common.Interfaces;
using Common.Models;
using Logic.User;
using TestUtilities;
using TestUtilities.CrudImplementations;

namespace Logic.Tests.User;

[TestFixture]
public class LoginUseCaseTests
{
    [SetUp]
    public void Setup()
    {
        var crudFactory = new TestCrudFactory();

        _userCrud = crudFactory.GetCrud<AppUser>();
        _passwordHasher = new TestPasswordHasher();
        _tokenHandler = new TestTokenHandler();
        _loginUseCase = new LoginUseCase(crudFactory, _passwordHasher, _tokenHandler);
    }

    private ICrud<AppUser> _userCrud;
    private TestPasswordHasher _passwordHasher;
    private TestTokenHandler _tokenHandler;
    private LoginUseCase _loginUseCase;

    [Test]
    public async Task LoginAsync_ShouldReturnFailure_WhenUserNotFound()
    {
        var request = new LoginRequest { Name = "NoUser", Password = "Password1" };
        var result = await _loginUseCase.LoginAsync(request);
        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
        Assert.That(result.ErrorMessage, Does.Contain("User not found"));
    }

    [Test]
    public async Task LoginAsync_ShouldReturnFailure_WhenPasswordIncorrect()
    {
        var user = new AppUser { Name = "TestUser", Password = _passwordHasher.HashPassword("Password1") };
        await _userCrud.CreateAsync(user);
        var request = new LoginRequest { Name = "TestUser", Password = "WrongPassword" };
        var result = await _loginUseCase.LoginAsync(request);
        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
        Assert.That(result.ErrorMessage, Does.Contain("Invalid password"));
    }

    [Test]
    public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsCorrect()
    {
        var user = new AppUser { Name = "TestUser", Password = _passwordHasher.HashPassword("Password1") };
        await _userCrud.CreateAsync(user);
        var request = new LoginRequest { Name = "TestUser", Password = "Password1" };
        var result = await _loginUseCase.LoginAsync(request);
        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result.Name, Is.EqualTo("TestUser"));
    }

    [Test]
    public async Task LoginAsync_ShouldReturnToken_WhenLoginSucceeds()
    {
        var user = new AppUser { Name = "TokenUser", Password = _passwordHasher.HashPassword("Password1") };
        await _userCrud.CreateAsync(user);
        var request = new LoginRequest { Name = "TokenUser", Password = "Password1" };
        var result = await _loginUseCase.LoginAsync(request);
        Assert.That(result.Result!.Token, Is.EqualTo("token-for-TokenUser"));
    }

    [Test]
    public async Task LoginAsync_ShouldReturnNullPassword_InUserResponse()
    {
        var user = new AppUser { Name = "NoPasswordUser", Password = _passwordHasher.HashPassword("Password1") };
        await _userCrud.CreateAsync(user);
        var request = new LoginRequest { Name = "NoPasswordUser", Password = "Password1" };
        var result = await _loginUseCase.LoginAsync(request);
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result.Password, Is.Null);
    }
}