using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Middleware;

namespace WebAPI.Controllers;

public abstract class ApplicationController(ICrudFactory factory) : Controller
{
    protected readonly ICrudFactory Factory = factory;

    protected string? CurrentUserId => HttpContext.Items[UserMiddleware.UserIdItemKey] as string;

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