using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace WebAPI.Integration.Tests.Controllers;

public abstract class ControllerTestBase : IDisposable
{
	private static bool HasInitialized { get; set; }
	private HttpClient Client { get; set; }
	private WebApplicationFactory<Program> Factory { get; set; }

	[TearDown]
	public void Dispose()
	{
		Client.Dispose();
		Factory.Dispose();
	}

	[SetUp]
	public void SetUpFactory()
	{
		Factory = new TestWebApplicationFactory();
		Client = Factory.CreateClient();
	}

	protected async Task<TResponse?> GetAsync<TResponse>(string url, bool includeToken = true)
	{
		var request = new HttpRequestMessage(HttpMethod.Get, url);
		if (includeToken)
		{
			var token = await GetLoginTokenAsync();
			request.Headers.Add("X-User-Token", token);
		}

		var response = await Client.SendAsync(request);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<TResponse>();
	}

	protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest body, bool includeToken = true)
	{
		var request = new HttpRequestMessage(HttpMethod.Post, url)
		{
			Content = JsonContent.Create(body)
		};
		if (includeToken)
		{
			var token = await GetLoginTokenAsync();
			request.Headers.Add("X-User-Token", token);
		}

		var response = await Client.SendAsync(request);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<TResponse>();
	}

	protected async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest body, bool includeToken = true)
	{
		var request = new HttpRequestMessage(HttpMethod.Put, url)
		{
			Content = JsonContent.Create(body)
		};
		if (includeToken)
		{
			var token = await GetLoginTokenAsync();
			request.Headers.Add("X-User-Token", token);
		}

		var response = await Client.SendAsync(request);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<TResponse>();
	}

	protected async Task<TResponse?> DeleteAsync<TResponse>(string url, bool includeToken = true)
	{
		var request = new HttpRequestMessage(HttpMethod.Delete, url);
		if (includeToken)
		{
			var token = await GetLoginTokenAsync();
			request.Headers.Add("X-User-Token", token);
		}

		var response = await Client.SendAsync(request);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<TResponse>();
	}

	protected async Task<HttpResponseMessage> GetRawAsync(string url, bool includeToken = true)
	{
		var request = new HttpRequestMessage(HttpMethod.Get, url);
		if (includeToken)
		{
			var token = await GetLoginTokenAsync();
			request.Headers.Add("X-User-Token", token);
		}

		return await Client.SendAsync(request);
	}

	protected async Task<HttpResponseMessage> PostRawAsync<TRequest>(string url,
		TRequest body,
		bool includeToken = true)
	{
		var request = new HttpRequestMessage(HttpMethod.Post, url)
		{
			Content = JsonContent.Create(body)
		};
		if (includeToken)
		{
			var token = await GetLoginTokenAsync();
			request.Headers.Add("X-User-Token", token);
		}

		return await Client.SendAsync(request);
	}

	private async Task<string> GetLoginTokenAsync()
	{
		if (HasInitialized)
		{
			var loginResponse = await MakeLogin();
			if (!string.IsNullOrEmpty(loginResponse))
				return loginResponse;

			throw new Exception("Failed to login test user.");
		}

		HasInitialized = true;
		//todo: implement user creation functionality
		return "pretend-this-is-a-token";
	}

	private async Task<string> MakeLogin()
	{
		if (!HasInitialized) await GetLoginTokenAsync();

		// todo: implement login functionality
		return "pretend-this-is-a-token";
	}

	private class TestWebApplicationFactory : WebApplicationFactory<Program>
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.UseEnvironment("Test");
		}
	}
}