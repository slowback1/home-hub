using Logic.FeatureFlags;

namespace Logic.Tests.FeatureFlags;

public class FeatureFlagServiceTests
{
    private Dictionary<string, bool> GetTestFeatureFlags()
    {
        return new Dictionary<string, bool>
        {
            { "Feature1", true },
            { "Feature2", false }
        };
    }

    [Test]
    [TestCase("Feature1", true)]
    [TestCase("Feature2", false)]
    [TestCase("Feature3", false)]
    public async Task FeatureIsEnabledReturnsExpectedValue(string featureFlag, bool expected)
    {
        var provider = new DictionaryFeatureFlagProvider(GetTestFeatureFlags());
        var service = new FeatureFlagService(provider);

        var result = await service.FeatureIsEnabled(featureFlag);

        Assert.That(result, Is.EqualTo(expected));
    }
}