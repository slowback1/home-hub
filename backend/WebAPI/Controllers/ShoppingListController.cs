using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/shopping-lists")]
public class ShoppingListController : ApplicationController
{
    private readonly ICrud<ShoppingList> _lists;

    public ShoppingListController(ICrudFactory factory) : base(factory)
    {
        _lists = Factory.GetCrud<ShoppingList>();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShoppingList>>> List()
    {
        if (CurrentUserId is null)
            return Unauthorized();

        var items = await _lists.QueryAsync(l => l.UserId == CurrentUserId && l.Status == ShoppingListStatus.Active);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingList>> Create([FromBody] CreateShoppingListRequest request)
    {
        if (CurrentUserId is null)
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Name is required.");

        var list = new ShoppingList
        {
            Id = Guid.NewGuid().ToString(),
            UserId = CurrentUserId,
            Name = request.Name,
            Status = ShoppingListStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _lists.CreateAsync(list);
        return Ok(created);
    }

    public record CreateShoppingListRequest(string? Name);
}
