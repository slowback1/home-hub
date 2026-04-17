using Common.Models;
using InMemory;

namespace Logic.Tests;

public class GetExampleDataUseCaseTests
{
    [Test]
    public async Task Execute_ReturnsExpectedExampleData()
    {
        var useCase = new GetExampleDataUseCase(new InMemoryGenericCrud<ExampleData>());

        var result = await useCase.Execute();

        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
        Assert.NotNull(result.Result);
        Assert.That(result.Result!.Id, Is.EqualTo("1"));
        Assert.That(result.Result.Name, Is.EqualTo("Example"));
        Assert.That(result.Result.Value, Is.EqualTo(42));
    }
}