using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class WheelControllerTests : ControllerTestBase
{
    [SetUp]
    public void ClearState()
    {
        InMemoryCrud<Wheel>.ClearStaticState();
    }

    private async Task<Wheel> CreateWheelAsync(string name, string items)
    {
        var response = await PostRawAsync("/api/wheels", new { Name = name, Items = items }, includeToken: false);
        return (await response.Content.ReadFromJsonAsync<Wheel>())!;
    }

    [Test]
    public async Task Post_CreatesWheel_Returns200_WithIdAndFields()
    {
        var before = DateTime.UtcNow;
        var response = await PostRawAsync(
            "/api/wheels",
            new { Name = "Dinner", Items = "Pizza\nTacos\nSushi" },
            includeToken: false);
        var after = DateTime.UtcNow;
        var created = await response.Content.ReadFromJsonAsync<Wheel>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(created.Name, Is.EqualTo("Dinner"));
        Assert.That(created.Items, Is.EqualTo("Pizza\nTacos\nSushi"));
        Assert.That(created.CreatedAt, Is.InRange(before, after));
    }

    [Test]
    public async Task Post_EmptyName_Returns400()
    {
        var response = await PostRawAsync(
            "/api/wheels",
            new { Name = "", Items = "Pizza" },
            includeToken: false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Post_WhitespaceName_Returns400()
    {
        var response = await PostRawAsync(
            "/api/wheels",
            new { Name = "   ", Items = "Pizza" },
            includeToken: false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Post_AllowsEmptyItems()
    {
        var response = await PostRawAsync(
            "/api/wheels",
            new { Name = "Empty", Items = "" },
            includeToken: false);
        var created = await response.Content.ReadFromJsonAsync<Wheel>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Items, Is.EqualTo(""));
    }

    [Test]
    public async Task Post_AllowsDuplicateNames()
    {
        await CreateWheelAsync("Dinner", "Pizza");
        var second = await PostRawAsync(
            "/api/wheels",
            new { Name = "Dinner", Items = "Tacos" },
            includeToken: false);

        Assert.That(second.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Get_NoWheels_ReturnsEmptyArray()
    {
        var response = await GetRawAsync("/api/wheels", includeToken: false);
        var wheels = await response.Content.ReadFromJsonAsync<Wheel[]>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(wheels, Is.Empty);
    }

    [Test]
    public async Task Get_ReturnsCreatedWheels()
    {
        await CreateWheelAsync("Dinner", "Pizza\nTacos");
        await CreateWheelAsync("Movies", "Alien\nHeat");

        var response = await GetRawAsync("/api/wheels", includeToken: false);
        var wheels = await response.Content.ReadFromJsonAsync<Wheel[]>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(wheels, Has.Length.EqualTo(2));
    }

    [Test]
    public async Task Put_UpdatesNameAndItems()
    {
        var created = await CreateWheelAsync("Dinner", "Pizza\nTacos");

        var response = await PutRawAsync(
            $"/api/wheels/{created.Id}",
            new { Name = "Dinner Options", Items = "Pizza\nTacos\nRamen" },
            includeToken: false);
        var updated = await response.Content.ReadFromJsonAsync<Wheel>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(updated!.Name, Is.EqualTo("Dinner Options"));
        Assert.That(updated.Items, Is.EqualTo("Pizza\nTacos\nRamen"));
    }

    [Test]
    public async Task Put_UnknownId_Returns404()
    {
        var response = await PutRawAsync(
            "/api/wheels/does-not-exist",
            new { Name = "Nope", Items = "x" },
            includeToken: false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Delete_RemovesWheel()
    {
        var created = await CreateWheelAsync("Dinner", "Pizza");

        var deleteResponse = await DeleteRawAsync($"/api/wheels/{created.Id}", includeToken: false);
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var list = await GetRawAsync("/api/wheels", includeToken: false);
        var wheels = await list.Content.ReadFromJsonAsync<Wheel[]>();
        Assert.That(wheels, Is.Empty);
    }

    [Test]
    public async Task Delete_UnknownId_Returns404()
    {
        var response = await DeleteRawAsync("/api/wheels/does-not-exist", includeToken: false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
