using Common.Interfaces;
using EntityFramework;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.Integration.Tests.SystemConfig;

public class SystemConfigDiTests
{
    [Test]
    public void ISystemConfigProvider_ResolvesSuccessfully_WhenEntityFrameworkIsConfigured()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase("SystemConfigDiTests"));

                    services.AddScoped<ISystemConfigProvider>(sp =>
                        new EfSystemConfigProvider(sp.GetRequiredService<AppDbContext>()));
                });
            });

        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetService<ISystemConfigProvider>();

        Assert.That(provider, Is.Not.Null);
        Assert.That(provider, Is.InstanceOf<EfSystemConfigProvider>());
    }
}
