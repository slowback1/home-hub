using Common.Interfaces;
using Common.Models;
using Logic;

namespace ConsoleUtilities.Handlers;

public class GetExampleDataHandler(ICrudFactory crudFactory) : BaseHandler(crudFactory)
{

    public override async Task HandleAsync(string[] args)
    {
        var useCase = new GetExampleDataUseCase(CrudFactory.GetCrud<ExampleData>());
        var data = await useCase.Execute();

        if (data.Result != null) Console.WriteLine($"{data.Result.Id} | {data.Result.Name} | {data.Result.Value}");
    }

    public override string GetHelpMessage()
    {
        return "Proof of concept handler to get example data.";
    }

    public override string GetName()
    {
        return "GetExampleData";
    }
}