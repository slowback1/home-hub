using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Interfaces;

public interface IFeatureFlagProvider
{
    Task<IEnumerable<FeatureFlag>> GetFeatureFlags();
}