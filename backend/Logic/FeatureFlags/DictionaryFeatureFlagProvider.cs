using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.FeatureFlags;

public class DictionaryFeatureFlagProvider(Dictionary<string, bool> featureFlags)
    : IFeatureFlagProvider, IFeatureFlagRepository
{
    public Task<IEnumerable<FeatureFlag>> GetFeatureFlags()
    {
        return Task.FromResult(featureFlags.Select(kvp => new FeatureFlag
        {
            Name = kvp.Key,
            IsEnabled = kvp.Value
        }));
    }

    public Task<FeatureFlag> UpdateAsync(string name, bool isEnabled)
    {
        if (!featureFlags.ContainsKey(name))
            throw new KeyNotFoundException($"Feature flag '{name}' not found.");

        featureFlags[name] = isEnabled;
        return Task.FromResult(new FeatureFlag { Name = name, IsEnabled = isEnabled });
    }
}