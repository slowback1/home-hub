using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class DashboardControllerTests : ControllerTestBase
{
    private record SlotDto(int SlotIndex, string? WidgetType);
    private record LayoutResponse(string LayoutFormat, List<SlotDto> Slots);
    private record SaveLayoutRequest(string LayoutFormat, List<SlotDto> Slots);

    [SetUp]
    public void ClearState()
    {
        InMemoryCrud<DashboardSlot>.ClearStaticState();
    }

    // --- GET /api/dashboard/layout ---

    [Test]
    public async Task GetLayout_ReturnsAllSixSlots_WhenNoneAssigned()
    {
        var response = await GetRawAsync("/api/dashboard/layout", includeToken: false);
        var layout = await response.Content.ReadFromJsonAsync<LayoutResponse>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(layout!.LayoutFormat, Is.EqualTo("3x2"));
        Assert.That(layout.Slots.Count, Is.EqualTo(6));
        Assert.That(layout.Slots.Select(s => s.SlotIndex), Is.EquivalentTo(Enumerable.Range(0, 6)));
        Assert.That(layout.Slots.All(s => s.WidgetType == null), Is.True);
    }

    [Test]
    public async Task GetLayout_ReturnsAssignedWidgets_InCorrectSlots()
    {
        var saveRequest = new SaveLayoutRequest("3x2",
        [
            new SlotDto(0, "tasks"),
            new SlotDto(2, "weather")
        ]);
        await PutRawAsync("/api/dashboard/layout", saveRequest);

        var response = await GetRawAsync("/api/dashboard/layout", includeToken: false);
        var layout = await response.Content.ReadFromJsonAsync<LayoutResponse>();

        Assert.That(layout!.Slots.Single(s => s.SlotIndex == 0).WidgetType, Is.EqualTo("tasks"));
        Assert.That(layout.Slots.Single(s => s.SlotIndex == 1).WidgetType, Is.Null);
        Assert.That(layout.Slots.Single(s => s.SlotIndex == 2).WidgetType, Is.EqualTo("weather"));
    }

    // --- PUT /api/dashboard/layout ---

    [Test]
    public async Task SaveLayout_Returns204()
    {
        var request = new SaveLayoutRequest("3x2", [new SlotDto(0, "tasks")]);
        var response = await PutRawAsync("/api/dashboard/layout", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task SaveLayout_FullReplace_RemovesSlotNotInPayload()
    {
        // First, assign tasks to slot 0 and weather to slot 1
        await PutRawAsync("/api/dashboard/layout", new SaveLayoutRequest("3x2",
        [
            new SlotDto(0, "tasks"),
            new SlotDto(1, "weather")
        ]));

        // Now PUT with only slot 0 — slot 1 should be cleared
        await PutRawAsync("/api/dashboard/layout", new SaveLayoutRequest("3x2",
        [
            new SlotDto(0, "tasks")
        ]));

        var response = await GetRawAsync("/api/dashboard/layout", includeToken: false);
        var layout = await response.Content.ReadFromJsonAsync<LayoutResponse>();

        Assert.That(layout!.Slots.Single(s => s.SlotIndex == 1).WidgetType, Is.Null);
    }

    [Test]
    public async Task SaveLayout_RoundTrip_PersistsAllAssignedWidgets()
    {
        var slots = new List<SlotDto>
        {
            new(0, "tasks"),
            new(1, "weather"),
            new(2, "activity"),
            new(3, "audiobook"),
            new(4, "retro"),
            new(5, "bookmarks")
        };

        await PutRawAsync("/api/dashboard/layout", new SaveLayoutRequest("3x2", slots));

        var response = await GetRawAsync("/api/dashboard/layout", includeToken: false);
        var layout = await response.Content.ReadFromJsonAsync<LayoutResponse>();

        foreach (var expected in slots)
        {
            var actual = layout!.Slots.Single(s => s.SlotIndex == expected.SlotIndex);
            Assert.That(actual.WidgetType, Is.EqualTo(expected.WidgetType));
        }
    }

    // --- DELETE /api/test/dashboard ---

    [Test]
    public async Task TestHelper_ClearDashboard_ClearsAllSlots()
    {
        await PutRawAsync("/api/dashboard/layout", new SaveLayoutRequest("3x2",
            [new SlotDto(0, "tasks")]));

        var clearResponse = await DeleteRawAsync("/api/test/dashboard");
        Assert.That(clearResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var response = await GetRawAsync("/api/dashboard/layout", includeToken: false);
        var layout = await response.Content.ReadFromJsonAsync<LayoutResponse>();
        Assert.That(layout!.Slots.All(s => s.WidgetType == null), Is.True);
    }
}
