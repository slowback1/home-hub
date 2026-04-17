using Common.Interfaces;
using Common.Models;
using Logic;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("ExampleData")]
public class ExampleDataController : ApplicationController
{
    private readonly ICrud<ExampleData> _exampleDataCrud;

    public ExampleDataController(ICrudFactory factory) : base(factory)
    {
        _exampleDataCrud = Factory.GetCrud<ExampleData>();
    }

    [HttpGet("")]
    public async Task<ActionResult> Get()
    {
        var useCase = new GetExampleDataUseCase(_exampleDataCrud);

        var result = await useCase.Execute();

        return ToActionResult(result);
    }
}