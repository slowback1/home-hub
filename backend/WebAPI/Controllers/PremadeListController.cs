using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/premade-lists")]
public class PremadeListController : ApplicationController
{
    private readonly ICrud<PremadeList> _lists;
    private readonly ICrud<PremadeListItem> _memberships;
    private readonly ICrud<SavedItem> _savedItems;

    public PremadeListController(ICrudFactory factory) : base(factory)
    {
        _lists = Factory.GetCrud<PremadeList>();
        _memberships = Factory.GetCrud<PremadeListItem>();
        _savedItems = Factory.GetCrud<SavedItem>();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PremadeListResponse>>> List()
    {
        if (CurrentUserId is null) return Unauthorized();

        var lists = await _lists.QueryAsync(l => l.UserId == CurrentUserId);

        var responses = new List<PremadeListResponse>();
        foreach (var list in lists)
            responses.Add(await ToResponse(list));

        return Ok(responses);
    }

    [HttpPost]
    public async Task<ActionResult<PremadeListResponse>> Create([FromBody] CreatePremadeListRequest request)
    {
        if (CurrentUserId is null) return Unauthorized();
        if (string.IsNullOrWhiteSpace(request.Name)) return BadRequest("Name is required.");

        var list = new PremadeList
        {
            Id = Guid.NewGuid().ToString(),
            UserId = CurrentUserId,
            Name = request.Name.Trim(),
            CreatedAt = DateTime.UtcNow,
        };

        var created = await _lists.CreateAsync(list);
        return Ok(await ToResponse(created));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        if (CurrentUserId is null) return Unauthorized();

        var list = await _lists.GetByIdAsync(id);
        if (list is null || list.UserId != CurrentUserId) return NotFound();

        var memberships = await _memberships.QueryAsync(m => m.PremadeListId == list.Id);
        foreach (var membership in memberships)
            await _memberships.DeleteAsync(membership.Id);

        await _lists.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/items")]
    public async Task<ActionResult<PremadeListResponse>> AddItem(string id, [FromBody] AddPremadeListItemRequest request)
    {
        if (CurrentUserId is null) return Unauthorized();
        if (string.IsNullOrWhiteSpace(request.SavedItemId)) return BadRequest("SavedItemId is required.");

        var list = await _lists.GetByIdAsync(id);
        if (list is null || list.UserId != CurrentUserId) return NotFound();

        var savedItem = await _savedItems.GetByIdAsync(request.SavedItemId);
        if (savedItem is null || savedItem.UserId != CurrentUserId) return NotFound();

        var existing = await _memberships.QueryAsync(
            m => m.PremadeListId == list.Id && m.SavedItemId == savedItem.Id);
        if (existing.Any()) return Conflict();

        var siblings = (await _memberships.QueryAsync(m => m.PremadeListId == list.Id)).ToList();
        var nextSortOrder = siblings.Count > 0 ? siblings.Max(m => m.SortOrder) + 1 : 0;

        await _memberships.CreateAsync(new PremadeListItem
        {
            Id = Guid.NewGuid().ToString(),
            PremadeListId = list.Id,
            SavedItemId = savedItem.Id,
            SortOrder = nextSortOrder,
        });

        return Ok(await ToResponse(list));
    }

    [HttpDelete("{id}/items/{savedItemId}")]
    public async Task<ActionResult> RemoveItem(string id, string savedItemId)
    {
        if (CurrentUserId is null) return Unauthorized();

        var list = await _lists.GetByIdAsync(id);
        if (list is null || list.UserId != CurrentUserId) return NotFound();

        var memberships = await _memberships.QueryAsync(
            m => m.PremadeListId == list.Id && m.SavedItemId == savedItemId);
        var membership = memberships.FirstOrDefault();
        if (membership is null) return NotFound();

        await _memberships.DeleteAsync(membership.Id);
        return NoContent();
    }

    [HttpPost("{id}/items/{savedItemId}/move")]
    public async Task<ActionResult<PremadeListResponse>> MoveItem(
        string id, string savedItemId, [FromBody] MovePremadeListItemRequest request)
    {
        if (CurrentUserId is null) return Unauthorized();

        var direction = request.Direction?.ToLowerInvariant();
        if (direction != "up" && direction != "down")
            return BadRequest("Direction must be 'up' or 'down'.");

        var list = await _lists.GetByIdAsync(id);
        if (list is null || list.UserId != CurrentUserId) return NotFound();

        var memberships = await _memberships.QueryAsync(m => m.PremadeListId == list.Id);
        var ordered = memberships
            .OrderBy(m => m.SortOrder)
            .ThenBy(m => m.CreatedAt)
            .ToList();

        var index = ordered.FindIndex(m => m.SavedItemId == savedItemId);
        if (index < 0) return NotFound();

        var target = direction == "up" ? index - 1 : index + 1;
        if (target >= 0 && target < ordered.Count)
        {
            var item = ordered[index];
            var neighbor = ordered[target];
            (item.SortOrder, neighbor.SortOrder) = (neighbor.SortOrder, item.SortOrder);
            await _memberships.UpdateAsync(item.Id, item);
            await _memberships.UpdateAsync(neighbor.Id, neighbor);
        }

        return Ok(await ToResponse(list));
    }

    private async Task<PremadeListResponse> ToResponse(PremadeList list)
    {
        var memberships = await _memberships.QueryAsync(m => m.PremadeListId == list.Id);
        return new PremadeListResponse
        {
            Id = list.Id,
            UserId = list.UserId,
            Name = list.Name,
            CreatedAt = list.CreatedAt,
            SavedItemIds = [.. memberships
                .OrderBy(m => m.SortOrder)
                .ThenBy(m => m.CreatedAt)
                .Select(m => m.SavedItemId)]
        };
    }

    public record CreatePremadeListRequest(string? Name);
    public record AddPremadeListItemRequest(string? SavedItemId);
    public record MovePremadeListItemRequest(string? Direction);
    public record PremadeListResponse
    {
        public required string Id { get; init; }
        public required string UserId { get; init; }
        public required string Name { get; init; }
        public required DateTime CreatedAt { get; init; }
        public required string[] SavedItemIds { get; init; }
    }
}
