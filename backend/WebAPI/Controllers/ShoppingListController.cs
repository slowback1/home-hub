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
    public async Task<ActionResult<IEnumerable<ShoppingListResponse>>> List([FromQuery] ShoppingListStatus? status = null)
    {
        if (CurrentUserId is null)
            return Unauthorized();

        var wanted = status ?? ShoppingListStatus.Active;
        var lists = await _lists.QueryAsync(l => l.UserId == CurrentUserId && l.Status == wanted);

        var responses = new List<ShoppingListResponse>();
        foreach (var list in lists)
            responses.Add(await ToResponse(list));

        return Ok(responses);
    }

    [HttpPost("{id}/complete")]
    public async Task<ActionResult<ShoppingListResponse>> Complete(string id)
    {
        if (CurrentUserId is null)
            return Unauthorized();

        var list = await _lists.GetByIdAsync(id);
        if (list is null || list.UserId != CurrentUserId)
            return NotFound();

        if (list.Status == ShoppingListStatus.Archived)
            return BadRequest("List is already archived.");

        list.Status = ShoppingListStatus.Archived;
        list.ArchivedAt = DateTime.UtcNow;
        await _lists.UpdateAsync(id, list);

        return Ok(await ToResponse(list));
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

        if (list.Status == ShoppingListStatus.Archived)
            return BadRequest("Cannot add items to an archived list.");

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

    [HttpPatch("{id}/items/{itemId}")]
    public async Task<ActionResult<ShoppingListItem>> ToggleItem(string id, string itemId)
    {
        if (CurrentUserId is null)
            return Unauthorized();

        var list = await _lists.GetByIdAsync(id);
        if (list is null || list.UserId != CurrentUserId)
            return NotFound();

        if (list.Status == ShoppingListStatus.Archived)
            return BadRequest("Cannot toggle items on an archived list.");

        var item = await _items.GetByIdAsync(itemId);
        if (item is null || item.ListId != list.Id)
            return NotFound();

        item.IsChecked = !item.IsChecked;
        var updated = await _items.UpdateAsync(itemId, item);
        return Ok(updated);
    }

    private async Task<ShoppingListResponse> ToResponse(ShoppingList list)
    {
        var items = await _items.QueryAsync(i => i.ListId == list.Id);
        return new ShoppingListResponse
        {
            Id = list.Id,
            UserId = list.UserId,
            Name = list.Name,
            Status = list.Status,
            ArchivedAt = list.ArchivedAt,
            CreatedAt = list.CreatedAt,
            Items = [.. items]
        };
    }

    public record CreateShoppingListRequest(string? Name);
    public record AddItemRequest(string? Text);
    public record ShoppingListResponse
    {
        public required string Id { get; init; }
        public required string UserId { get; init; }
        public required string Name { get; init; }
        public required ShoppingListStatus Status { get; init; }
        public required DateTime? ArchivedAt { get; init; }
        public required DateTime CreatedAt { get; init; }
        public required ShoppingListItem[] Items { get; init; }
    }
}
