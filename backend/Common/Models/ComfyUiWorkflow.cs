using Common.Interfaces;

namespace Common.Models;

public class ComfyUiWorkflow : IIdentifyable
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string WorkflowJson { get; set; } = "";
}
