using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Common.Interfaces;
using Common.Models;
using Microsoft.IdentityModel.Tokens;

namespace Logic.User;

public class TokenHandler(TokenGeneratorConfig config) : ITokenHandler
{
	public string GenerateToken(AppUser user)
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.SecretKey));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, user.Id),
			new Claim(JwtRegisteredClaimNames.UniqueName, user.Name)
		};
		var token = new JwtSecurityToken(
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(config.ExpiryMinutes),
			signingCredentials: creds
		);
		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	public AppUser ReadToken(string token)
	{
		var handler = new JwtSecurityTokenHandler();
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.SecretKey));
		var parameters = new TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = key
		};
		try
		{
			handler.ValidateToken(token, parameters, out var validatedToken);
			var jwt = (JwtSecurityToken)validatedToken;

			if (jwt.ValidTo < DateTime.UtcNow)
				throw new TokenExpiredException("Token has expired.");

			var id = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
			var name = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;

			return new AppUser
			{
				Id = id ?? string.Empty,
				Name = name ?? string.Empty
			};
		}
		catch (TokenExpiredException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new TokenInvalidException("Token is invalid.", ex);
		}
	}
}