using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.FeatureFlags;

public class DictionaryFeatureFlagProvider(Dictionary<string, bool> featureFlags) : IFeatureFlagProvider
{
    public Task<IEnumerable<FeatureFlag>> GetFeatureFlags()
    {
        return Task.FromResult(featureFlags.Select(kvp => new FeatureFlag
        {
            Name = kvp.Key,
            IsEnabled = kvp.Value
        }));
    }
}