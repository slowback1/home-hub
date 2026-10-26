using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/users")]
public class UserController : ApplicationController
{
    private readonly ICrud<AppUser> _users;

    public UserController(ICrudFactory factory) : base(factory)
    {
        _users = Factory.GetCrud<AppUser>();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUserResponse>>> List()
    {
        var users = await _users.QueryAsync(_ => true);
        return Ok(users.Select(u => new AppUserResponse { Id = u.Id, Name = u.Name }));
    }

    public record AppUserResponse
    {
        public required string Id { get; init; }
        public required string Name { get; init; }
    }
}
