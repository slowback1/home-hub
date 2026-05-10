using System;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/comfyui")]
public class ComfyUiController : ApplicationController
{
    private readonly ICrud<ComfyUiWorkflow> _workflows;
    private readonly IComfyUiClient _comfyUiClient;

    public ComfyUiController(ICrudFactory factory, IComfyUiClient comfyUiClient) : base(factory)
    {
        _workflows = Factory.GetCrud<ComfyUiWorkflow>();
        _comfyUiClient = comfyUiClient;
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

    [HttpPost("generate")]
    public async Task<ActionResult<GenerateResponse>> Generate([FromBody] GenerateRequest request)
    {
        var workflow = await _workflows.GetByIdAsync(request.WorkflowId);
        if (workflow is null)
            return NotFound();

        var workflowJson = workflow.WorkflowJson
            .Replace("{{prompt}}", request.Prompt)
            .Replace("{{negative_prompt}}", request.NegativePrompt ?? "");

        try
        {
            var promptId = await _comfyUiClient.SubmitPromptAsync(workflowJson);
            var imageBytes = await _comfyUiClient.PollForImageAsync(promptId);
            var base64 = Convert.ToBase64String(imageBytes);
            return Ok(new GenerateResponse($"data:image/png;base64,{base64}"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"ComfyUI generation failed: {ex.Message}");
        }
    }

    public record WorkflowListItem(string Id, string Name);
    public record CreateWorkflowRequest(string Name, string WorkflowJson);
    public record GenerateRequest(string WorkflowId, string Prompt, string? NegativePrompt = null);
    public record GenerateResponse(string ImageBase64);
}
