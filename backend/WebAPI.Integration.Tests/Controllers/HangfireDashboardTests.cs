namespace WebAPI.Integration.Tests.Controllers;

public class HangfireDashboardTests : ControllerTestBase
{
    [Test]
    public async Task HangfireDashboard_IsAccessible()
    {
        var response = await GetRawAsync("/admin/hangfire", includeToken: false);
        Assert.That((int)response.StatusCode, Is.InRange(200, 399));
    }
}
