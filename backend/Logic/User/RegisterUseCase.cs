using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.User;

public class RegisterUseCase(ICrud<AppUser> userCrud, IPasswordHasher passwordHasher, ITokenHandler tokenHandler)
{
	public async Task<UseCaseResult<UserResponse>> RegisterAsync(RegisterRequest request)
	{
		var validationResult = await ValidateRegistration(request);
		if (validationResult != null)
			return validationResult;

		var user = await CreateUser(request);

		return CreateToken(user);
	}

	private UseCaseResult<UserResponse> CreateToken(AppUser user)
	{
		var token = tokenHandler.GenerateToken(user);
		var response = new UserResponse
		{
			Id = user.Id,
			Name = user.Name,
			Token = token
		};
		return UseCaseResult<UserResponse>.Success(response);
	}

	private async Task<AppUser> CreateUser(RegisterRequest request)
	{
		var hashedPassword = passwordHasher.HashPassword(request.Password);
		var user = new AppUser { Name = request.Name, Password = hashedPassword };
		await userCrud.CreateAsync(user);
		return user;
	}

	private async Task<UseCaseResult<UserResponse>?> ValidateRegistration(RegisterRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length < 4)
			return UseCaseResult<UserResponse>.Failure("Username must be at least 4 characters");

		if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
			return UseCaseResult<UserResponse>.Failure("Password must be at least 8 characters");
		if (!Regex.IsMatch(request.Password, "[A-Z]"))
			return UseCaseResult<UserResponse>.Failure("Password must contain at least one uppercase letter");
		if (!Regex.IsMatch(request.Password, "[a-z]"))
			return UseCaseResult<UserResponse>.Failure("Password must contain at least one lowercase letter");

		if (request.Password != request.ConfirmPassword)
			return UseCaseResult<UserResponse>.Failure("Passwords do not match");

		var existing = await userCrud.GetByQueryAsync(u => u.Name == request.Name);
		if (existing != null)
			return UseCaseResult<UserResponse>.Failure("A user with this username already exists");

		return null;
	}
}