using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/shopping-lists")]
public class ShoppingListController : ApplicationController
{
    private readonly ICrud<ShoppingList> _lists;
    private readonly ICrud<ShoppingListItem> _items;

    public ShoppingListController(ICrudFactory factory) : base(factory)
    {
        _lists = Factory.GetCrud<ShoppingList>();
        _items = Factory.GetCrud<ShoppingListItem>();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShoppingListResponse>>> List()
    {
        if (CurrentUserId is null)
            return Unauthorized();

        var lists = await _lists.QueryAsync(l => l.UserId == CurrentUserId && l.Status == ShoppingListStatus.Active);

        var responses = new List<ShoppingListResponse>();
        foreach (var list in lists)
        {
            var items = await _items.QueryAsync(i => i.ListId == list.Id);
            responses.Add(new ShoppingListResponse(
                list.Id, list.UserId, list.Name, list.Status, list.ArchivedAt, list.CreatedAt,
                [.. items]));
        }

        return Ok(responses);
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

    [HttpPost("{id}/items")]
    public async Task<ActionResult<ShoppingListItem>> AddItem(string id, [FromBody] AddItemRequest request)
    {
        if (CurrentUserId is null)
            return Unauthorized();

        var list = await _lists.GetByIdAsync(id);
        if (list is null || list.UserId != CurrentUserId)
            return NotFound();

        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest("Text is required.");

        var item = new ShoppingListItem
        {
            Id = Guid.NewGuid().ToString(),
            ListId = list.Id,
            Text = request.Text,
            IsChecked = false,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _items.CreateAsync(item);
        return Ok(created);
    }

    public record CreateShoppingListRequest(string? Name);
    public record AddItemRequest(string? Text);
    public record ShoppingListResponse(
        string Id, string UserId, string Name, ShoppingListStatus Status,
        DateTime? ArchivedAt, DateTime CreatedAt, ShoppingListItem[] Items);
}
