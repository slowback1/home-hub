using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

public class HealthCheckResult
{
    public string Status { get; init; } = string.Empty;
    public DateTime ResponseDate { get; set; } = DateTime.Now;
}

[Route("HealthCheck")]
public class HealthCheckController(ICrudFactory factory) : ApplicationController(factory)
{

    [HttpGet]
    [Route("")]
    public ActionResult HealthCheck()
    {
        return Ok(new HealthCheckResult { Status = "Healthy" });
    }
}