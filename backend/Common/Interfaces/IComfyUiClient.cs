using System.Threading.Tasks;

namespace Common.Interfaces;

public interface IComfyUiClient
{
    Task<string> SubmitPromptAsync(string workflowJson);
    Task<byte[]> PollForImageAsync(string promptId);
}
