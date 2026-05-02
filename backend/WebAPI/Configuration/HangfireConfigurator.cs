using Hangfire;
using Hangfire.Dashboard;
using Hangfire.InMemory;
using Hangfire.PostgreSql;

namespace WebAPI.Configuration;

public static class HangfireConfigurator
{
    public static void ConfigureHangfire(IServiceCollection services, IConfiguration configuration)
    {
        var crudImpl = configuration["CrudFactory:Implementation"]?.ToLower() ?? "inmemory";

        services.AddHangfire(config =>
        {
            if (crudImpl == "entityframework")
            {
                var connStr = configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Hangfire requires DefaultConnection when using EntityFramework.");
                config.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connStr));
            }
            else
            {
                config.UseInMemoryStorage();
            }
        });

        services.AddHangfireServer();
    }

    public static DashboardOptions DashboardOptions => new()
    {
        // No auth guard — home network only. Add IDashboardAuthorizationFilter before internet exposure.
        Authorization = [new AllowAllDashboardFilter()]
    };

    private sealed class AllowAllDashboardFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context) => true;
    }
}
