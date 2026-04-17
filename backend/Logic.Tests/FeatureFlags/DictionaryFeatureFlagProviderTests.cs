using Logic.FeatureFlags;

namespace Logic.Tests.FeatureFlags;

public class DictionaryFeatureFlagProviderTests
{
    [Test]
    public async Task GetFeatureFlagsReturnsAnEmptyListWhenGivenNothing()
    {
        var provider = new DictionaryFeatureFlagProvider(new Dictionary<string, bool>());

        var result = await provider.GetFeatureFlags();

        Assert.IsEmpty(result);
    }

    [Test]
    public async Task GetFeatureFlagsReturnsGivenFeatureFlags()
    {
        var featureFlags = new Dictionary<string, bool>
        {
            { "Feature1", true },
            { "Feature2", false }
        };
        var provider = new DictionaryFeatureFlagProvider(featureFlags);

        var result = (await provider.GetFeatureFlags()).ToList();

        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.First().Name, Is.EqualTo("Feature1"));
        Assert.That(result.First().IsEnabled, Is.EqualTo(true));
        Assert.That(result.Last().Name, Is.EqualTo("Feature2"));
        Assert.That(result.Last().IsEnabled, Is.EqualTo(false));
    }
}