using Common.Interfaces;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework;

public class EfSystemConfigProvider(AppDbContext context) : ISystemConfigProvider
{
    public async Task<SystemConfig> GetAsync(string @namespace, string key)
    {
        var entry = await FindAsync(@namespace, key);
        return entry.IsSecret ? Mask(entry) : entry;
    }

    public Task<SystemConfig> GetSecretAsync(string @namespace, string key) =>
        FindAsync(@namespace, key);

    public async Task<IEnumerable<SystemConfig>> GetAllAsync()
    {
        return await context.SystemConfigs
            .Include(c => c.Options)
            .ToListAsync();
    }

    public async Task<SystemConfig> UpdateAsync(string @namespace, string key, string value)
    {
        var entry = await FindAsync(@namespace, key);
        entry.Value = value;
        await context.SaveChangesAsync();
        return entry.IsSecret ? Mask(entry) : entry;
    }

    private async Task<SystemConfig> FindAsync(string @namespace, string key)
    {
        var id = $"{@namespace}::{key}";
        return await context.SystemConfigs
            .Include(c => c.Options)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new KeyNotFoundException($"System config key '{id}' was not found.");
    }

    private static SystemConfig Mask(SystemConfig entry) =>
        new()
        {
            Id = entry.Id,
            Namespace = entry.Namespace,
            Key = entry.Key,
            Value = "***",
            Type = entry.Type,
            IsSecret = entry.IsSecret,
            Options = entry.Options
        };
}
