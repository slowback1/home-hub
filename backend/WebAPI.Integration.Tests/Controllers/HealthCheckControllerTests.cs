using WebAPI.Controllers;

namespace WebAPI.Integration.Tests.Controllers;

public class HealthCheckControllerTests : ControllerTestBase
{
    [Test]
    public async Task GetHealthCheck_ReturnsOk()
    {
        var url = "/healthcheck";
        var response = await GetAsync<HealthCheckResult>(url);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Status, Is.EqualTo("Healthy"));
    }
}