using EntityFramework;
using Microsoft.EntityFrameworkCore;
using SC = Common.Models.SystemConfig;

namespace WebAPI.Integration.Tests.SystemConfig;

public class EfSystemConfigProviderTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static SC MakeEntry(string @namespace, string key, string value, bool isSecret = false) =>
        new()
        {
            Id = $"{@namespace}::{key}",
            Namespace = @namespace,
            Key = key,
            Value = value,
            Type = "string",
            IsSecret = isSecret
        };

    private static async Task<EfSystemConfigProvider> CreateProvider(AppDbContext ctx, params SC[] seed)
    {
        ctx.SystemConfigs.AddRange(seed);
        await ctx.SaveChangesAsync();
        return new EfSystemConfigProvider(new EfCrudFactory(ctx));
    }

    [Test]
    public async Task GetAsync_ReturnsEntry_WhenKeyExists()
    {
        using var ctx = CreateContext();
        var provider = await CreateProvider(ctx, MakeEntry("general", "site_name", "HomeHub"));

        var result = await provider.GetAsync("general", "site_name");

        Assert.That(result.Value, Is.EqualTo("HomeHub"));
    }

    [Test]
    public async Task GetAsync_MasksValue_WhenEntryIsSecret()
    {
        using var ctx = CreateContext();
        var provider = await CreateProvider(ctx, MakeEntry("weather", "api_key", "real-key", isSecret: true));

        var result = await provider.GetAsync("weather", "api_key");

        Assert.That(result.Value, Is.EqualTo("***"));
    }

    [Test]
    public void GetAsync_Throws_WhenKeyNotFound()
    {
        using var ctx = CreateContext();
        var provider = new EfSystemConfigProvider(new EfCrudFactory(ctx));

        Assert.ThrowsAsync<KeyNotFoundException>(() => provider.GetAsync("general", "missing"));
    }

    [Test]
    public async Task GetSecretAsync_ReturnsRealValue_WhenEntryIsSecret()
    {
        using var ctx = CreateContext();
        var provider = await CreateProvider(ctx, MakeEntry("weather", "api_key", "real-key", isSecret: true));

        var result = await provider.GetSecretAsync("weather", "api_key");

        Assert.That(result.Value, Is.EqualTo("real-key"));
    }

    [Test]
    public void GetSecretAsync_Throws_WhenKeyNotFound()
    {
        using var ctx = CreateContext();
        var provider = new EfSystemConfigProvider(new EfCrudFactory(ctx));

        Assert.ThrowsAsync<KeyNotFoundException>(() => provider.GetSecretAsync("general", "missing"));
    }

    [Test]
    public async Task GetAllAsync_ReturnsMaskedSecrets()
    {
        using var ctx = CreateContext();
        var provider = await CreateProvider(ctx,
            MakeEntry("general", "site_name", "HomeHub"),
            MakeEntry("weather", "api_key", "real-key", isSecret: true));

        var results = (await provider.GetAllAsync()).ToList();

        Assert.That(results.Count, Is.EqualTo(2));
        Assert.That(results.First(e => e.Key == "site_name").Value, Is.EqualTo("HomeHub"));
        Assert.That(results.First(e => e.Key == "api_key").Value, Is.EqualTo("***"));
    }

    [Test]
    public async Task UpdateAsync_PersistsNewValue_AndReturnsMaskedEntry_WhenSecret()
    {
        using var ctx = CreateContext();
        var provider = await CreateProvider(ctx, MakeEntry("weather", "api_key", "old-key", isSecret: true));

        var result = await provider.UpdateAsync("weather", "api_key", "new-key");

        Assert.That(result.Value, Is.EqualTo("***"));
        var fetched = await provider.GetSecretAsync("weather", "api_key");
        Assert.That(fetched.Value, Is.EqualTo("new-key"));
    }

    [Test]
    public async Task UpdateAsync_PersistsNewValue_AndReturnsEntry_WhenNotSecret()
    {
        using var ctx = CreateContext();
        var provider = await CreateProvider(ctx, MakeEntry("general", "site_name", "OldName"));

        var result = await provider.UpdateAsync("general", "site_name", "NewName");

        Assert.That(result.Value, Is.EqualTo("NewName"));
    }
}
