using Common.Interfaces;
using Hangfire;
using Logic.NbaSchedule;
using Logic.SystemConfig;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.Integration.Tests.Configuration;

public class NbaScheduleRecurringJobTests
{
    private static WebApplicationFactory<Program> BuildFactory() =>
        new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseEnvironment("Test"));

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
            recurringJob.Any(kv => kv.Key.Equals("cron", StringComparison.OrdinalIgnoreCase) && kv.Value == "0 3 * * *"),
            Is.True, "Expected the nba-schedule recurring job to run nightly at 03:00");
    }

    [Test]
    public void INbaScheduleProvider_DefaultsToMock_WhenConfigKeyMissing()
    {
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                    services.AddSingleton<ISystemConfigProvider>(
                        new DictionarySystemConfigProvider([])));
            });

        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<INbaScheduleProvider>();

        Assert.That(provider, Is.InstanceOf<MockNbaScheduleProvider>());
    }
}
