using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.User;

public class LoginUseCase(ICrudFactory crudFactory, IPasswordHasher passwordHasher, ITokenHandler tokenHandler)
{
	private readonly ICrud<AppUser> _userCrud = crudFactory.GetCrud<AppUser>();

	public async Task<UseCaseResult<UserResponse>> LoginAsync(LoginRequest request)
	{
		var user = await _userCrud.GetByQueryAsync(u => u.Name == request.Name);
		if (user == null)
			return UseCaseResult<UserResponse>.Failure("User not found");

		if (user.Password == null || !passwordHasher.VerifyPassword(request.Password, user.Password))
			return UseCaseResult<UserResponse>.Failure("Invalid password");

		var token = tokenHandler.GenerateToken(user);
		var response = new UserResponse
		{
			Id = user.Id,
			Name = user.Name,
			Token = token
		};
		return UseCaseResult<UserResponse>.Success(response);
	}
}