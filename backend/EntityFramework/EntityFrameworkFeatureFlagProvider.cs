using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework;

public class EntityFrameworkFeatureFlagProvider(AppDbContext db) : IFeatureFlagProvider
{
    public async Task<IEnumerable<FeatureFlag>> GetFeatureFlags() =>
        await db.FeatureFlags.ToListAsync();
}
