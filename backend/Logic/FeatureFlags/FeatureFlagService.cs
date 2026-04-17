using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.FeatureFlags;

public class FeatureFlagService(IFeatureFlagProvider featureFlagProvider)
{
    public async Task<bool> FeatureIsEnabled(string featureFlag)
    {
        var featureFlags = (await GetFeatureFlags()).ToList();

        return await FeatureExists(featureFlags, featureFlag) && await FeatureIsEnabled(featureFlags, featureFlag);
    }

    private async Task<IEnumerable<FeatureFlag>> GetFeatureFlags()
    {
        return await featureFlagProvider.GetFeatureFlags();
    }

    private async Task<bool> FeatureExists(IEnumerable<FeatureFlag> featureFlags, string feature)
    {
        return await Task.FromResult(featureFlags.Any(ff => ff.Name == feature));
    }

    private async Task<bool> FeatureIsEnabled(IEnumerable<FeatureFlag> featureFlags, string feature)
    {
        return await Task.FromResult(featureFlags.Any(ff => ff.Name == feature && ff.IsEnabled));
    }
}