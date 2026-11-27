using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.Integration.Tests.Configuration;

public class NbaScheduleRecurringJobTests
{
	private static WebApplicationFactory<Program> BuildFactory()
	{
		return new WebApplicationFactory<Program>()
			.WithWebHostBuilder(builder => builder.UseEnvironment("Test"));
	}

	[Test]
	public void Program_RegistersNbaScheduleRecurringJob()
	{
		using var factory = BuildFactory();
		factory.CreateClient();

		var storage = factory.Services.GetRequiredService<JobStorage>();
		using var connection = storage.GetConnection();
		var recurringJob = connection.GetAllEntriesFromHash("recurring-job:nba-schedule");

		Assert.That(recurringJob, Is.Not.Null,
			"Expected an nba-schedule recurring job to be registered");
		Assert.That(
			recurringJob.Any(kv =>
				kv.Key.Equals("cron", StringComparison.OrdinalIgnoreCase) && kv.Value == "0 3 * * *"),
			Is.True, "Expected the nba-schedule recurring job to run nightly at 03:00");
	}
}