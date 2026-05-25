using Common.Interfaces;
using Common.Models;
using Logic.Audiobook;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// Test-only endpoints for seeding and clearing state. Returns 404 in Production.
/// </summary>
[Route("api/test")]
public class TestHelperController(
    IActivityPickRepository pickRepository,
    ICrudFactory crudFactory,
    MockAudiobookService audiobookMock,
    IWebHostEnvironment env) : ControllerBase
{
    private bool IsAllowed =>
        env.IsEnvironment("Test") || env.IsEnvironment("E2E") || env.IsDevelopment();

    [HttpPost("activity-picks")]
    public async Task<ActionResult> SeedActivityPick([FromBody] SeedPickRequest request)
    {
        if (!IsAllowed) return NotFound();

        await pickRepository.WriteAsync(new ActivityPick
        {
            ActivityName = request.ActivityName,
            PickedAt = request.PickedAt
        });
        return Ok();
    }

    [HttpDelete("activity-picks")]
    public async Task<ActionResult> ClearActivityPicks()
    {
        if (!IsAllowed) return NotFound();

        await pickRepository.ClearAllAsync();
        return NoContent();
    }

    [HttpDelete("activities")]
    public async Task<ActionResult> ClearActivities()
    {
        if (!IsAllowed) return NotFound();

        var crud = crudFactory.GetCrud<Activity>();
        var all = await crud.QueryAsync(_ => true);
        foreach (var activity in all)
            await crud.DeleteAsync(activity.Id);

        return NoContent();
    }

    [HttpDelete("audiobook")]
    public async Task<ActionResult> ResetAudiobookMock()
    {
        if (!IsAllowed) return NotFound();
        await audiobookMock.ResetAsync();
        return NoContent();
    }

    [HttpDelete("comfyui")]
    public async Task<ActionResult> ClearComfyUiWorkflows()
    {
        if (!IsAllowed) return NotFound();

        var crud = crudFactory.GetCrud<ComfyUiWorkflow>();
        var all = await crud.QueryAsync(_ => true);
        foreach (var workflow in all)
            await crud.DeleteAsync(workflow.Id);

        return NoContent();
    }

    [HttpDelete("tasks")]
    public async Task<ActionResult> ClearTasks()
    {
        if (!IsAllowed) return NotFound();

        var taskCrud = crudFactory.GetCrud<ChoreTask>();
        var completionCrud = crudFactory.GetCrud<TaskCompletion>();

        var completions = await completionCrud.QueryAsync(_ => true);
        foreach (var c in completions)
            await completionCrud.DeleteAsync(c.Id);

        var tasks = await taskCrud.QueryAsync(_ => true);
        foreach (var t in tasks)
            await taskCrud.DeleteAsync(t.Id);

        return NoContent();
    }

    public record SeedPickRequest(string ActivityName, DateTime PickedAt);
}
