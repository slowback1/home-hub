using System;
using System.Threading.Tasks;
using Logic.Weather;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/weather")]
public class WeatherController(WeatherService weatherService) : ControllerBase
{
    [HttpGet("current")]
    public async Task<ActionResult> GetCurrent()
    {
        try
        {
            var conditions = await weatherService.GetCurrentAsync();
            return Ok(conditions);
        }
        catch (Exception ex)
        {
            return StatusCode(503, ex.Message);
        }
    }
}
