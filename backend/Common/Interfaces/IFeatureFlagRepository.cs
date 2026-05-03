using System.Threading.Tasks;
using Common.Models;

namespace Common.Interfaces;

public interface IFeatureFlagRepository
{
    Task<FeatureFlag> UpdateAsync(string name, bool isEnabled);
}
