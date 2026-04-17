using Common.Models;

namespace WebAPI.Integration.Tests.Controllers;

public class ExampleDataControllerTests : ControllerTestBase
{
    [Test]
    public async Task GetExampleData_ReturnsOk()
    {
        var url = "/exampledata";
        var response = await GetAsync<ExampleData>(url);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Id, Is.EqualTo("1"));
        Assert.That(response.Name, Is.EqualTo("Example"));
        Assert.That(response.Value, Is.EqualTo(42));
    }
}