using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WebAPI.Configuration;

namespace WebAPI.Integration.Tests.Configuration;

public class CorsConfiguratorTests
{
    private static CorsPolicy GetPolicy(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        CorsConfigurator.ConfigureCors(services, configuration);
        var provider = services.BuildServiceProvider();
        var corsOptions = provider.GetRequiredService<IOptions<CorsOptions>>().Value;
        return corsOptions.GetPolicy(CorsConfigurator.PolicyName)!;
    }

    private static IConfiguration BuildConfig(
        string[]? origins = null,
        string[]? methods = null,
        string[]? headers = null)
    {
        var dict = new Dictionary<string, string?>();

        if (origins is not null)
            for (var i = 0; i < origins.Length; i++)
                dict[$"Cors:AllowedOrigins:{i}"] = origins[i];

        if (methods is not null)
            for (var i = 0; i < methods.Length; i++)
                dict[$"Cors:AllowedMethods:{i}"] = methods[i];

        if (headers is not null)
            for (var i = 0; i < headers.Length; i++)
                dict[$"Cors:AllowedHeaders:{i}"] = headers[i];

        return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
    }

    [Test]
    public void WildcardOrigins_AllowsAnyOrigin()
    {
        var policy = GetPolicy(BuildConfig(origins: ["*"], methods: ["*"], headers: ["*"]));
        Assert.That(policy.AllowAnyOrigin, Is.True);
    }

    [Test]
    public void SpecificOrigins_RestrictsToThoseOrigins()
    {
        var policy = GetPolicy(BuildConfig(
            origins: ["https://example.com", "https://other.com"],
            methods: ["*"],
            headers: ["*"]));

        Assert.That(policy.AllowAnyOrigin, Is.False);
        Assert.That(policy.Origins, Does.Contain("https://example.com"));
        Assert.That(policy.Origins, Does.Contain("https://other.com"));
    }

    [Test]
    public void WildcardMethods_AllowsAnyMethod()
    {
        var policy = GetPolicy(BuildConfig(origins: ["*"], methods: ["*"], headers: ["*"]));
        Assert.That(policy.AllowAnyMethod, Is.True);
    }

    [Test]
    public void SpecificMethods_RestrictsToThoseMethods()
    {
        var policy = GetPolicy(BuildConfig(
            origins: ["*"],
            methods: ["GET", "POST"],
            headers: ["*"]));

        Assert.That(policy.AllowAnyMethod, Is.False);
        Assert.That(policy.Methods, Does.Contain("GET"));
        Assert.That(policy.Methods, Does.Contain("POST"));
    }

    [Test]
    public void WildcardHeaders_AllowsAnyHeader()
    {
        var policy = GetPolicy(BuildConfig(origins: ["*"], methods: ["*"], headers: ["*"]));
        Assert.That(policy.AllowAnyHeader, Is.True);
    }

    [Test]
    public void SpecificHeaders_RestrictsToThoseHeaders()
    {
        var policy = GetPolicy(BuildConfig(
            origins: ["*"],
            methods: ["*"],
            headers: ["Content-Type", "Authorization"]));

        Assert.That(policy.AllowAnyHeader, Is.False);
        Assert.That(policy.Headers, Does.Contain("Content-Type"));
        Assert.That(policy.Headers, Does.Contain("Authorization"));
    }

    [Test]
    public void EmptyConfig_TreatsAllSectionsAsWildcard()
    {
        var policy = GetPolicy(BuildConfig());
        Assert.That(policy.AllowAnyOrigin, Is.True);
        Assert.That(policy.AllowAnyMethod, Is.True);
        Assert.That(policy.AllowAnyHeader, Is.True);
    }
}
