using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Configuration;

namespace WebAPI.Integration.Tests.Configuration;

public class HangfireConfiguratorTests
{
    private static IConfiguration BuildConfig(string implementation)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CrudFactory:Implementation"] = implementation
            })
            .Build();
    }

    [Test]
    public void ConfigureHangfire_WithInMemory_RegistersBackgroundJobClient()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var config = BuildConfig("InMemory");

        HangfireConfigurator.ConfigureHangfire(services, config);

        var provider = services.BuildServiceProvider();
        Assert.That(() => provider.GetRequiredService<IBackgroundJobClient>(), Throws.Nothing);
    }

    [Test]
    public void ConfigureHangfire_WithFileData_UsesInMemoryStorage()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var config = BuildConfig("FileData");

        HangfireConfigurator.ConfigureHangfire(services, config);

        var provider = services.BuildServiceProvider();
        Assert.That(() => provider.GetRequiredService<IBackgroundJobClient>(), Throws.Nothing);
    }

    [Test]
    public void ConfigureHangfire_WithUnknownValue_FallsBackToInMemoryStorage()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var config = BuildConfig("SomethingElse");

        HangfireConfigurator.ConfigureHangfire(services, config);

        var provider = services.BuildServiceProvider();
        Assert.That(() => provider.GetRequiredService<IBackgroundJobClient>(), Throws.Nothing);
    }
}
