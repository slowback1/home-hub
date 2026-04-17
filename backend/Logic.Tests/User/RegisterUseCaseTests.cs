using Common.Interfaces;
using Common.Models;
using Logic.User;
using TestUtilities;
using TestUtilities.CrudImplementations;

namespace Logic.Tests.User;

[TestFixture]
public class RegisterUseCaseTests
{
    [SetUp]
    public void Setup()
    {
        var crudFactory = new TestCrudFactory();
        UserCrud = crudFactory.GetCrud<AppUser>();
        _passwordHasher = new TestPasswordHasher();
        _tokenHandler = new TestTokenHandler();
        _registerUseCase = new RegisterUseCase(UserCrud, _passwordHasher, _tokenHandler);
    }

    private ICrud<AppUser> UserCrud { get; set; }
    private RegisterUseCase _registerUseCase;
    private TestPasswordHasher _passwordHasher;
    private TestTokenHandler _tokenHandler;

    [Test]
    public async Task RegisterAsync_ShouldCreateUser_WhenValidRequest()
    {
        var request = new RegisterRequest { Name = "TestUser", Password = "Password1", ConfirmPassword = "Password1" };

        var result = await _registerUseCase.RegisterAsync(request);

        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result.Name, Is.EqualTo("TestUser"));
        Assert.That(result.Result.Token, Is.EqualTo("token-for-TestUser"));
    }

    [TestCase("a", "Password1", "Password1", "Username must be at least 4 characters")]
    [TestCase("ValidUser", "pass", "pass", "Password must be at least 8 characters")]
    [TestCase("ValidUser", "password", "password", "Password must contain at least one uppercase letter")]
    [TestCase("ValidUser", "PASSWORD", "PASSWORD", "Password must contain at least one lowercase letter")]
    [TestCase("ValidUser", "Password1", "Password2", "Passwords do not match")]
    public async Task RegisterAsync_ShouldReturnError_WhenValidationFails(string name,
        string password,
        string confirmPassword,
        string expectedError)
    {
        var request = new RegisterRequest { Name = name, Password = password, ConfirmPassword = confirmPassword };
        var result = await _registerUseCase.RegisterAsync(request);
        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
        Assert.That(result.ErrorMessage, Does.Contain(expectedError));
    }

    [Test]
    public async Task RegisterAsync_ShouldReturnError_WhenUserAlreadyExists()
    {
        var request = new RegisterRequest
            { Name = "ExistingUser", Password = "Password1", ConfirmPassword = "Password1" };
        await UserCrud.CreateAsync(new AppUser { Name = "ExistingUser", Password = "hashed" });
        var result = await _registerUseCase.RegisterAsync(request);
        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
        Assert.That(result.ErrorMessage, Does.Contain("already exists"));
    }

    [Test]
    public async Task RegisterAsync_ShouldHashPassword_WhenSavingUser()
    {
        var request = new RegisterRequest { Name = "HashUser", Password = "Password1", ConfirmPassword = "Password1" };
        var response = await _registerUseCase.RegisterAsync(request);
        var user = await UserCrud.GetByIdAsync(response.Result!.Id);
        Assert.That(user, Is.Not.Null);
        Assert.That(user.Password, Is.EqualTo("hashed:Password1"));
    }

    [Test]
    public async Task RegisterAsync_ShouldReturnToken_WhenRegistrationSucceeds()
    {
        var request = new RegisterRequest { Name = "TokenUser", Password = "Password1", ConfirmPassword = "Password1" };
        var result = await _registerUseCase.RegisterAsync(request);
        Assert.That(result.Result!.Token, Is.EqualTo("token-for-TokenUser"));
    }
}