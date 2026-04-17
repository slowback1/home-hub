using Common.Interfaces;
using FileData;
using InMemory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Configuration;

namespace WebAPI.Integration.Tests.Configuration;

public class CrudFactoryConfiguratorTests
{
    [Test]
    public void ConfigureCrudFactory_WithInMemory_RegistersInMemoryCrudFactory()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration("InMemory");

        CrudFactoryConfigurator.ConfigureCrudFactory(services, configuration);

        var serviceProvider = services.BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<ICrudFactory>();

        Assert.That(factory, Is.InstanceOf<InMemoryCrudFactory>());
    }

    [Test]
    public void ConfigureCrudFactory_WithFileData_RegistersFileCrudFactory()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration("FileData");

        CrudFactoryConfigurator.ConfigureCrudFactory(services, configuration);

        var serviceProvider = services.BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<ICrudFactory>();

        Assert.That(factory, Is.InstanceOf<FileCrudFactory>());
    }

    [Test]
    public void ConfigureCrudFactory_WithFileDataAndCustomDirectory_UsesCustomDirectory()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration("FileData", "/custom/path");

        CrudFactoryConfigurator.ConfigureCrudFactory(services, configuration);

        var serviceProvider = services.BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<ICrudFactory>() as FileCrudFactory;

        Assert.That(factory, Is.Not.Null);
        Assert.That(factory, Is.InstanceOf<FileCrudFactory>());
    }

    [Test]
    public void ConfigureCrudFactory_WithEnumOverride_UsesEnumValue()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration("FileData");

        CrudFactoryConfigurator.ConfigureCrudFactory(services, configuration,
            CrudFactoryConfigurator.CrudFactoryType.InMemory);

        var serviceProvider = services.BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<ICrudFactory>();

        Assert.That(factory, Is.InstanceOf<InMemoryCrudFactory>());
    }

    private IConfiguration CreateConfiguration(string? implementation, string? dataDirectory = null)
    {
        var configDict = new Dictionary<string, string?>();

        if (!string.IsNullOrEmpty(implementation)) configDict["CrudFactory:Implementation"] = implementation;

        if (!string.IsNullOrEmpty(dataDirectory)) configDict["FileData:Directory"] = dataDirectory;

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();
    }
}