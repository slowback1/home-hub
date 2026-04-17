using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;
using Logic.User;

namespace Logic.Authorization;

public class UserAuthorizationUseCase(ICrudFactory crudFactory, TokenGeneratorConfig tokenGeneratorConfig)
{
    private readonly ITokenHandler _tokenHandler = new TokenHandler(tokenGeneratorConfig);

    public async Task<UseCaseResult<AuthorizationResult>> AuthorizeAsync(string token)
    {
        var result = new AuthorizationResult();
        if (string.IsNullOrWhiteSpace(token))
        {
            result.Status = AuthorizationStatus.InvalidCredentials;
            result.ErrorMessage = "Token is missing.";
            return UseCaseResult<AuthorizationResult>.Success(result);
        }

        AppUser? userFromToken;
        try
        {
            userFromToken = _tokenHandler.ReadToken(token);
        }
        catch (TokenExpiredException ex)
        {
            result.Status = AuthorizationStatus.Error;
            result.ErrorMessage = ex.Message;
            return UseCaseResult<AuthorizationResult>.Success(result);
        }
        catch (TokenInvalidException ex)
        {
            result.Status = AuthorizationStatus.InvalidCredentials;
            result.ErrorMessage = ex.Message;
            return UseCaseResult<AuthorizationResult>.Success(result);
        }
        catch (Exception ex)
        {
            result.Status = AuthorizationStatus.Error;
            result.ErrorMessage = ex.Message;
            return UseCaseResult<AuthorizationResult>.Success(result);
        }

        var userCrud = crudFactory.GetCrud<AppUser>();
        var user = await userCrud.GetByQueryAsync(u => u.Name == userFromToken.Name);
        if (user == null)
        {
            result.Status = AuthorizationStatus.UserNotFound;
            result.ErrorMessage = "User not found.";
            return UseCaseResult<AuthorizationResult>.Success(result);
        }

        result.Status = AuthorizationStatus.Authenticated;
        result.UserId = user.Id;
        result.User = user;
        result.Token = token;
        return UseCaseResult<AuthorizationResult>.Success(result);
    }

    public string GetTokenFromHeaders(IDictionary<string, string>? headers)
    {
        if (headers is null) return string.Empty;

        var lowerHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var header in headers) lowerHeaders[header.Key.ToLower()] = header.Value;

        if (!lowerHeaders.TryGetValue("x-user-token", out var token) || string.IsNullOrWhiteSpace(token))
            return string.Empty;
        return token;
    }
}