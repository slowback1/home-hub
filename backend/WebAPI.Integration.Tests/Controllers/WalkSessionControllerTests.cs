using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class WalkSessionControllerTests : ControllerTestBase
{
    private record CreateRequest(
        string? ClientId,
        DateTime StartedAt,
        int DurationSeconds,
        int StepCount);

    [SetUp]
    public void ClearState()
    {
        InMemoryCrud<WalkSession>.ClearStaticState();
    }

    [Test]
    public async Task Post_CreatesSession_AndReturns200()
    {
        var request = new CreateRequest(
            ClientId: Guid.NewGuid().ToString(),
            StartedAt: new DateTime(2026, 5, 28, 10, 0, 0, DateTimeKind.Utc),
            DurationSeconds: 600,
            StepCount: 1000);

        var response = await PostRawAsync("/api/walk-sessions", request, includeToken: false);
        var created = await response.Content.ReadFromJsonAsync<WalkSession>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Id, Is.Not.Empty);
        Assert.That(created.ClientId, Is.EqualTo(request.ClientId));
        Assert.That(created.StepCount, Is.EqualTo(1000));
        Assert.That(created.DurationSeconds, Is.EqualTo(600));
    }

    [Test]
    public async Task Post_SetsIdAndSyncedAt_Ignoring_ClientSupplied()
    {
        var request = new CreateRequest(
            ClientId: Guid.NewGuid().ToString(),
            StartedAt: new DateTime(2026, 5, 28, 10, 0, 0, DateTimeKind.Utc),
            DurationSeconds: 300,
            StepCount: 500);

        var before = DateTime.UtcNow;
        var response = await PostRawAsync("/api/walk-sessions", request, includeToken: false);
        var after = DateTime.UtcNow;
        var created = await response.Content.ReadFromJsonAsync<WalkSession>();

        Assert.That(created!.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(created.SyncedAt, Is.InRange(before, after));
    }

    [Test]
    public async Task Post_SameClientId_Returns200_WithOriginalRecord()
    {
        var clientId = Guid.NewGuid().ToString();
        var request = new CreateRequest(
            ClientId: clientId,
            StartedAt: new DateTime(2026, 5, 28, 10, 0, 0, DateTimeKind.Utc),
            DurationSeconds: 600,
            StepCount: 1000);

        var first = await PostRawAsync("/api/walk-sessions", request, includeToken: false);
        var firstRecord = await first.Content.ReadFromJsonAsync<WalkSession>();

        var second = await PostRawAsync("/api/walk-sessions", request, includeToken: false);
        var secondRecord = await second.Content.ReadFromJsonAsync<WalkSession>();

        Assert.That(second.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(secondRecord!.Id, Is.EqualTo(firstRecord!.Id));
    }

    [Test]
    public async Task Post_MissingClientId_Returns400()
    {
        var request = new CreateRequest(
            ClientId: "",
            StartedAt: new DateTime(2026, 5, 28, 10, 0, 0, DateTimeKind.Utc),
            DurationSeconds: 600,
            StepCount: 1000);

        var response = await PostRawAsync("/api/walk-sessions", request, includeToken: false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Post_NegativeDuration_Returns400()
    {
        var request = new CreateRequest(
            ClientId: Guid.NewGuid().ToString(),
            StartedAt: new DateTime(2026, 5, 28, 10, 0, 0, DateTimeKind.Utc),
            DurationSeconds: -1,
            StepCount: 500);

        var response = await PostRawAsync("/api/walk-sessions", request, includeToken: false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Post_NegativeStepCount_Returns400()
    {
        var request = new CreateRequest(
            ClientId: Guid.NewGuid().ToString(),
            StartedAt: new DateTime(2026, 5, 28, 10, 0, 0, DateTimeKind.Utc),
            DurationSeconds: 600,
            StepCount: -1);

        var response = await PostRawAsync("/api/walk-sessions", request, includeToken: false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Post_MinDateStartedAt_Returns400()
    {
        var request = new CreateRequest(
            ClientId: Guid.NewGuid().ToString(),
            StartedAt: default,
            DurationSeconds: 600,
            StepCount: 500);

        var response = await PostRawAsync("/api/walk-sessions", request, includeToken: false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Get_NoSessions_ReturnsEmptyArray()
    {
        var response = await GetRawAsync("/api/walk-sessions", includeToken: false);
        var sessions = await response.Content.ReadFromJsonAsync<WalkSession[]>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(sessions, Is.Empty);
    }

    [Test]
    public async Task Get_MultipleSessions_ReturnsSortedByStartedAtDescending()
    {
        var older = new CreateRequest(
            ClientId: Guid.NewGuid().ToString(),
            StartedAt: new DateTime(2026, 5, 20, 8, 0, 0, DateTimeKind.Utc),
            DurationSeconds: 600,
            StepCount: 1000);

        var newer = new CreateRequest(
            ClientId: Guid.NewGuid().ToString(),
            StartedAt: new DateTime(2026, 5, 25, 9, 0, 0, DateTimeKind.Utc),
            DurationSeconds: 900,
            StepCount: 2000);

        await PostRawAsync("/api/walk-sessions", older, includeToken: false);
        await PostRawAsync("/api/walk-sessions", newer, includeToken: false);

        var response = await GetRawAsync("/api/walk-sessions", includeToken: false);
        var sessions = await response.Content.ReadFromJsonAsync<WalkSession[]>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(sessions, Has.Length.EqualTo(2));
        Assert.That(sessions![0].StartedAt, Is.GreaterThan(sessions[1].StartedAt));
    }
}
