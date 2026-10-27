using System.Net;
using System.Net.Http.Json;
using Common.Interfaces;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class DashboardLayoutUserScopeTests : ControllerTestBase
{
    private record SlotDto(int SlotIndex, string? WidgetType);
    private record LayoutResponse(string LayoutFormat, List<SlotDto> Slots);

    private string _userA = string.Empty;
    private string _userB = string.Empty;

    [SetUp]
    public async Task ClearState()
    {
        InMemoryCrud<DashboardSlot>.ClearStaticState();
        InMemoryCrud<AppUser>.ClearStaticState();
        _userA = await SeedTestUserAsync("Dash User A");
        _userB = await SeedTestUserAsync("Dash User B");
    }

    private async Task SaveLayoutAsAsync(string userId, params (int SlotIndex, string? WidgetType)[] slots)
    {
        var response = await SendAsUserAsync(HttpMethod.Put, "/api/dashboard/layout",
            new
            {
                LayoutFormat = "3x2",
                Slots = slots.Select(s => new { s.SlotIndex, s.WidgetType }).ToList()
            },
            userId);
        response.EnsureSuccessStatusCode();
    }

    [Test]
    public async Task GetLayout_WithoutUserHeader_Returns401()
    {
        var response = await GetRawAsync("/api/dashboard/layout", includeToken: false);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task SaveLayout_WithoutUserHeader_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Put, "/api/dashboard/layout",
            new { LayoutFormat = "3x2", Slots = new[] { new { SlotIndex = 0, WidgetType = "tasks" } } });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetLayout_UserWithNoSavedRows_ReturnsSixEmptySlots()
    {
        await SaveLayoutAsAsync(_userA, (0, "tasks"));

        var response = await GetRawAsUserAsync("/api/dashboard/layout", _userB);
        var layout = await response.Content.ReadFromJsonAsync<LayoutResponse>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(layout!.Slots.Count, Is.EqualTo(6));
        Assert.That(layout.Slots.Select(s => s.SlotIndex), Is.EquivalentTo(Enumerable.Range(0, 6)));
        Assert.That(layout.Slots.All(s => s.WidgetType == null), Is.True);
    }

    [Test]
    public async Task SaveLayout_UserADoesNotChangeUserBLayout()
    {
        await SaveLayoutAsAsync(_userA, (0, "tasks"), (2, "weather"));

        var response = await GetRawAsUserAsync("/api/dashboard/layout", _userB);
        var layout = await response.Content.ReadFromJsonAsync<LayoutResponse>();

        Assert.That(layout!.Slots.Single(s => s.SlotIndex == 0).WidgetType, Is.Null);
        Assert.That(layout.Slots.Single(s => s.SlotIndex == 2).WidgetType, Is.Null);
    }

    [Test]
    public async Task SaveLayout_SlotsComeBackForSameUser()
    {
        await SaveLayoutAsAsync(_userA, (0, "tasks"), (1, "weather"));

        var response = await GetRawAsUserAsync("/api/dashboard/layout", _userA);
        var layout = await response.Content.ReadFromJsonAsync<LayoutResponse>();

        Assert.That(layout!.Slots.Single(s => s.SlotIndex == 0).WidgetType, Is.EqualTo("tasks"));
        Assert.That(layout.Slots.Single(s => s.SlotIndex == 1).WidgetType, Is.EqualTo("weather"));
    }

    [Test]
    public async Task SaveLayout_StampsUserIdOnCreatedRows()
    {
        await SaveLayoutAsAsync(_userA, (0, "tasks"));

        ICrud<DashboardSlot> crud = new InMemoryGenericCrud<DashboardSlot>();
        var rows = (await crud.QueryAsync(s => s.LayoutFormat == "3x2")).ToList();

        Assert.That(rows, Is.Not.Empty);
        Assert.That(rows.All(s => s.UserId == _userA), Is.True);
    }
}
