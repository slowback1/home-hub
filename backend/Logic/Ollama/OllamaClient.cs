using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Common.Interfaces;

namespace Logic.Ollama;

public class OllamaClient(HttpClient httpClient, ISystemConfigProvider configProvider) : IOllamaClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private async Task<string> GetBaseUrlAsync()
    {
        var entry = await configProvider.GetAsync("ollama", "url");
        return entry.Value.TrimEnd('/');
    }

    public async Task<string> ChatAsync(string model, string prompt)
    {
        var baseUrl = await GetBaseUrlAsync();
        var requestBody = new ChatRequest
        {
            Model = model,
            Messages = [new ChatMessage { Role = "user", Content = prompt }],
            Stream = false
        };
        var json = JsonSerializer.Serialize(requestBody, JsonOptions);
        var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/api/chat")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var chatResponse = JsonSerializer.Deserialize<ChatResponse>(responseJson, JsonOptions)
            ?? throw new InvalidOperationException("Empty response from Ollama.");
        return chatResponse.Message.Content;
    }

    private sealed class ChatRequest
    {
        public string Model { get; set; } = string.Empty;
        public ChatMessage[] Messages { get; set; } = [];
        public bool Stream { get; set; }
    }

    private sealed class ChatMessage
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    private sealed class ChatResponse
    {
        public ChatMessage Message { get; set; } = new();
    }
}
