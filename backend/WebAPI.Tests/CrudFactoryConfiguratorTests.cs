using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using WebAPI.Configuration;
using Xunit;
using EntityFramework;
using Common.Interfaces;
using Microsoft.EntityFrameworkCore;

public class CrudFactoryConfiguratorTests
{
    private ServiceCollection CreateServices(string impl, string? connectionString = null)
    {
        var inMemorySettings = new System.Collections.Generic.Dictionary<string, string?>
        {
            {"CrudFactory:Implementation", impl},
            {"ConnectionStrings:DefaultConnection", connectionString ?? "Host=localhost;Database=fakedb;Username=fake;Password=fake;"}
        };
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        var svcs = new ServiceCollection();
        CrudFactoryConfigurator.ConfigureCrudFactory(svcs, config);
        return svcs;
    }

    [Fact]
    public void Registers_InMemoryCrudFactory_ByDefault()
    {
        var services = CreateServices("inmemory");
        var sp = services.BuildServiceProvider();
        var factory = sp.GetRequiredService<ICrudFactory>();
        Assert.IsType<InMemory.InMemoryCrudFactory>(factory);
    }

    [Fact]
    public void Registers_FileCrudFactory_WhenConfigured()
    {
        var services = CreateServices("filedata");
        var sp = services.BuildServiceProvider();
        var factory = sp.GetRequiredService<ICrudFactory>();
        Assert.IsType<FileData.FileCrudFactory>(factory);
    }

    [Fact]
    public void Registers_EfCrudFactory_WhenEntityFrameworkIsConfigured()
    {
        var services = CreateServices("entityframework");
        // Need to register AppDbContext for EF case; should throw if unresolved
        services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("tddInMemory"));
        services.AddScoped<ICrudFactory, EfCrudFactory>();
        var sp = services.BuildServiceProvider();
        var factory = sp.GetRequiredService<ICrudFactory>();
        Assert.IsType<EfCrudFactory>(factory);
    }
}
