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
		FileData,
		EntityFramework
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
			factoryType?.ToString() ?? configuration["CrudFactory:Implementation"] ?? "InMemory";

		switch (crudFactoryImpl.ToLower())
		{
			case "filedata":
				var dataDirectory = configuration["FileData:Directory"] ?? "data";
				services.AddScoped<ICrudFactory>(_ => new FileCrudFactory(dataDirectory));
				break;
			case "entityframework":
				// EF Core registration is handled in Program.cs alongside AppDbContext.
				// This case is a hook for future wiring; InMemory is used as a safe fallback
				// until the EntityFramework project is registered.
				services.AddScoped<ICrudFactory, InMemoryCrudFactory>();
				break;
			default:
				services.AddScoped<ICrudFactory, InMemoryCrudFactory>();
				break;
		}
	}
}