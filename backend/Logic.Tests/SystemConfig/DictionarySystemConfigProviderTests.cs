using Logic.SystemConfig;
using SC = Common.Models.SystemConfig;

namespace Logic.Tests.SystemConfig;

public class DictionarySystemConfigProviderTests
{
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

    [Test]
    public async Task GetAsync_ReturnsEntry_WhenKeyExists()
    {
        var entry = MakeEntry("general", "site_name", "HomeHub");
        var provider = new DictionarySystemConfigProvider([entry]);

        var result = await provider.GetAsync("general", "site_name");

        Assert.That(result.Value, Is.EqualTo("HomeHub"));
    }

    [Test]
    public async Task GetAsync_MasksValue_WhenEntryIsSecret()
    {
        var entry = MakeEntry("weather", "api_key", "real-secret-key", isSecret: true);
        var provider = new DictionarySystemConfigProvider([entry]);

        var result = await provider.GetAsync("weather", "api_key");

        Assert.That(result.Value, Is.EqualTo("***"));
    }

    [Test]
    public void GetAsync_Throws_WhenKeyNotFound()
    {
        var provider = new DictionarySystemConfigProvider([]);

        Assert.ThrowsAsync<KeyNotFoundException>(() => provider.GetAsync("general", "missing"));
    }

    [Test]
    public async Task GetSecretAsync_ReturnsRealValue_WhenEntryIsSecret()
    {
        var entry = MakeEntry("weather", "api_key", "real-secret-key", isSecret: true);
        var provider = new DictionarySystemConfigProvider([entry]);

        var result = await provider.GetSecretAsync("weather", "api_key");

        Assert.That(result.Value, Is.EqualTo("real-secret-key"));
    }

    [Test]
    public void GetSecretAsync_Throws_WhenKeyNotFound()
    {
        var provider = new DictionarySystemConfigProvider([]);

        Assert.ThrowsAsync<KeyNotFoundException>(() => provider.GetSecretAsync("general", "missing"));
    }

    [Test]
    public async Task GetAllAsync_ReturnsMaskedSecrets()
    {
        var entries = new[]
        {
            MakeEntry("general", "site_name", "HomeHub"),
            MakeEntry("weather", "api_key", "real-key", isSecret: true)
        };
        var provider = new DictionarySystemConfigProvider(entries);

        var results = (await provider.GetAllAsync()).ToList();

        Assert.That(results.Count, Is.EqualTo(2));
        Assert.That(results.First(e => e.Key == "site_name").Value, Is.EqualTo("HomeHub"));
        Assert.That(results.First(e => e.Key == "api_key").Value, Is.EqualTo("***"));
    }

    [Test]
    public async Task UpdateAsync_UpdatesValueAndReturnsMaskedEntry_WhenSecret()
    {
        var entry = MakeEntry("weather", "api_key", "old-key", isSecret: true);
        var provider = new DictionarySystemConfigProvider([entry]);

        var result = await provider.UpdateAsync("weather", "api_key", "new-key");

        Assert.That(result.Value, Is.EqualTo("***"));
        var fetched = await provider.GetSecretAsync("weather", "api_key");
        Assert.That(fetched.Value, Is.EqualTo("new-key"));
    }

    [Test]
    public async Task UpdateAsync_UpdatesValueAndReturnsEntry_WhenNotSecret()
    {
        var entry = MakeEntry("general", "site_name", "OldName");
        var provider = new DictionarySystemConfigProvider([entry]);

        var result = await provider.UpdateAsync("general", "site_name", "NewName");

        Assert.That(result.Value, Is.EqualTo("NewName"));
    }
}
