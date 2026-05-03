using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.FeatureFlags;

public class UpdateFeatureFlagUseCase(IFeatureFlagRepository repository)
{
    public Task<FeatureFlag> ExecuteAsync(string name, bool isEnabled) =>
        repository.UpdateAsync(name, isEnabled);
}
