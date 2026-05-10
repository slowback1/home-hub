using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/comfyui")]
public class ComfyUiController : ApplicationController
{
    private readonly ICrud<ComfyUiWorkflow> _workflows;

    public ComfyUiController(ICrudFactory factory) : base(factory)
    {
        _workflows = Factory.GetCrud<ComfyUiWorkflow>();
    }

    [HttpGet("workflows")]
    public async Task<ActionResult<IEnumerable<WorkflowListItem>>> GetWorkflows()
    {
        var workflows = await _workflows.QueryAsync(_ => true);
        return Ok(workflows.Select(w => new WorkflowListItem(w.Id, w.Name)));
    }

    [HttpPost("workflows")]
    public async Task<ActionResult<WorkflowListItem>> CreateWorkflow([FromBody] CreateWorkflowRequest request)
    {
        var workflow = new ComfyUiWorkflow
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            WorkflowJson = request.WorkflowJson
        };
        var created = await _workflows.CreateAsync(workflow);
        return Ok(new WorkflowListItem(created.Id, created.Name));
    }

    [HttpDelete("workflows/{id}")]
    public async Task<ActionResult> DeleteWorkflow(string id)
    {
        var deleted = await _workflows.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }

    public record WorkflowListItem(string Id, string Name);
    public record CreateWorkflowRequest(string Name, string WorkflowJson);
}
