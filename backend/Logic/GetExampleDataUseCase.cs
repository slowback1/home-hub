using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic;

public class GetExampleDataUseCase(ICrud<ExampleData> crud)
{
    private readonly ICrud<ExampleData> _crud = crud;

    public Task<UseCaseResult<ExampleData>> Execute()
    {
        var data = new ExampleData
        {
            Id = "1",
            Name = "Example",
            Value = 42
        };

        return Task.FromResult(UseCaseResult<ExampleData>.Success(data));
    }
}