using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

public abstract class ApplicationController(ICrudFactory factory) : Controller
{
    protected readonly ICrudFactory Factory = factory;

    protected ActionResult ToActionResult<T>(UseCaseResult<T> result)
    {
        return result.Status switch
        {
            UseCaseStatus.Success => Ok(result.Result),
            UseCaseStatus.Failure => StatusCode(400, new { error = result.ErrorMessage }),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}