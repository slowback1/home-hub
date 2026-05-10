using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Common.Interfaces;

namespace Logic.ComfyUi;

public class ComfyUiClient(
    HttpClient httpClient,
    ISystemConfigProvider configProvider,
    int pollIntervalMs = 1000) : IComfyUiClient
{
    private async Task<string> GetBaseUrlAsync()
    {
        var entry = await configProvider.GetAsync("comfyui", "base_url");
        return entry.Value.TrimEnd('/');
    }

    public async Task<string> SubmitPromptAsync(string workflowJson)
    {
        var baseUrl = await GetBaseUrlAsync();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/prompt")
        {
            Content = new StringContent(workflowJson, Encoding.UTF8, "application/json")
        };

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var node = JsonNode.Parse(json) ?? throw new InvalidOperationException("Empty response from ComfyUI.");
        return node["prompt_id"]!.GetValue<string>();
    }

    public async Task<byte[]> PollForImageAsync(string promptId)
    {
        var baseUrl = await GetBaseUrlAsync();

        while (true)
        {
            var response = await httpClient.GetAsync($"{baseUrl}/history/{promptId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);

            var promptNode = node?[promptId];
            if (promptNode != null)
            {
                var image = promptNode["outputs"]!.AsObject()
                    .First().Value!["images"]![0]!;

                var filename = image["filename"]!.GetValue<string>();
                var subfolder = image["subfolder"]!.GetValue<string>();
                var type = image["type"]!.GetValue<string>();

                var imageUrl = $"{baseUrl}/view?filename={Uri.EscapeDataString(filename)}&subfolder={Uri.EscapeDataString(subfolder)}&type={Uri.EscapeDataString(type)}";
                var imageResponse = await httpClient.GetAsync(imageUrl);
                imageResponse.EnsureSuccessStatusCode();
                return await imageResponse.Content.ReadAsByteArrayAsync();
            }

            await Task.Delay(pollIntervalMs);
        }
    }
}
