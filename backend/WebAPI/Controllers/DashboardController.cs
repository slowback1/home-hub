using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/dashboard")]
public class DashboardController : ApplicationController
{
    private const string ActiveFormat = "3x2";
    private const int SlotCount = 6;
    private readonly ICrud<DashboardSlot> _slots;

    public DashboardController(ICrudFactory factory) : base(factory)
    {
        _slots = Factory.GetCrud<DashboardSlot>();
    }

    [HttpGet("layout")]
    public async Task<ActionResult<LayoutResponse>> GetLayout()
    {
        var existing = await _slots.QueryAsync(s => s.LayoutFormat == ActiveFormat);
        var slotMap = existing.ToDictionary(s => s.SlotIndex);

        var slots = Enumerable.Range(0, SlotCount)
            .Select(i => new SlotDto(i, slotMap.TryGetValue(i, out var s) ? s.WidgetType : null))
            .ToList();

        return Ok(new LayoutResponse(ActiveFormat, slots));
    }

    [HttpPut("layout")]
    public async Task<ActionResult> SaveLayout([FromBody] SaveLayoutRequest request)
    {
        var existing = await _slots.QueryAsync(s => s.LayoutFormat == ActiveFormat);
        var existingByIndex = existing.ToDictionary(s => s.SlotIndex);
        var incomingByIndex = request.Slots.ToDictionary(s => s.SlotIndex);

        foreach (var slot in existing.Where(s => !incomingByIndex.ContainsKey(s.SlotIndex)))
            await _slots.DeleteAsync(slot.Id);

        foreach (var dto in request.Slots)
        {
            if (existingByIndex.TryGetValue(dto.SlotIndex, out var existingSlot))
            {
                existingSlot.WidgetType = dto.WidgetType;
                await _slots.UpdateAsync(existingSlot.Id, existingSlot);
            }
            else
            {
                await _slots.CreateAsync(new DashboardSlot
                {
                    Id = Guid.NewGuid().ToString(),
                    LayoutFormat = ActiveFormat,
                    SlotIndex = dto.SlotIndex,
                    WidgetType = dto.WidgetType
                });
            }
        }

        return NoContent();
    }

    public record LayoutResponse(string LayoutFormat, List<SlotDto> Slots);
    public record SlotDto(int SlotIndex, string? WidgetType);
    public record SaveLayoutRequest(string LayoutFormat, List<SlotDto> Slots);
}
