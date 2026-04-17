using Common.Interfaces;
using FileData;
using InMemory;

namespace WebAPI.Configuration;

/// <summary>
///     Handles the configuration and registration of CRUD factory implementations
///     based on application configuration.
/// </summary>
public static class CrudFactoryConfigurator
{
	public enum CrudFactoryType
	{
		InMemory,
		FileData
	}

	/// <summary>
	///     Registers the appropriate CRUD factory implementation with the service collection.
	/// </summary>
	/// <param name="services">The service collection to configure</param>
	/// <param name="configuration">Application configuration</param>
	/// <param name="factoryType">Optional override for the factory type, otherwise reads from configuration</param>
	public static void ConfigureCrudFactory(IServiceCollection services,
		IConfiguration configuration,
		CrudFactoryType? factoryType = null)
	{
		var crudFactoryImpl =
			factoryType?.ToString() ?? configuration["CrudFactory:Implementation"] ?? "EntityFramework";

		switch (crudFactoryImpl.ToLower())
		{
			case "filedata":
				var dataDirectory = configuration["FileData:Directory"] ?? "data";
				services.AddSingleton<ICrudFactory>(_ => new FileCrudFactory(dataDirectory));
				break;
			default:
				services.AddSingleton<ICrudFactory, InMemoryCrudFactory>();
				break;
		}
	}
}