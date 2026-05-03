using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace EntityFramework;

public class EntityFrameworkFeatureFlagRepository(AppDbContext db) : IFeatureFlagRepository
{
    public async Task<FeatureFlag> UpdateAsync(string name, bool isEnabled)
    {
        var flag = await db.FeatureFlags.FindAsync(name)
            ?? throw new KeyNotFoundException($"Feature flag '{name}' not found.");

        flag.IsEnabled = isEnabled;
        await db.SaveChangesAsync();
        return flag;
    }
}
