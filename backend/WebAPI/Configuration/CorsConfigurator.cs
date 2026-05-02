namespace WebAPI.Configuration;

public static class CorsConfigurator
{
    public const string PolicyName = "ConfiguredCors";

    public static void ConfigureCors(IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        var methods  = configuration.GetSection("Cors:AllowedMethods").Get<string[]>() ?? [];
        var headers  = configuration.GetSection("Cors:AllowedHeaders").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                if (IsWildcard(origins))
                    policy.AllowAnyOrigin();
                else
                    policy.WithOrigins(origins);

                if (IsWildcard(methods))
                    policy.AllowAnyMethod();
                else
                    policy.WithMethods(methods);

                if (IsWildcard(headers))
                    policy.AllowAnyHeader();
                else
                    policy.WithHeaders(headers);
            });
        });
    }

    private static bool IsWildcard(string[] values) =>
        values.Length == 0 || (values.Length == 1 && values[0] == "*");
}
