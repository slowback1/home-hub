using Common.Interfaces;
using Common.Models;

namespace EntityFramework;

public class EfSystemConfigProvider(ICrudFactory crudFactory) : ISystemConfigProvider
{
    private readonly ICrud<SystemConfig> _crud = crudFactory.GetCrud<SystemConfig>();

    public async Task<SystemConfig> GetAsync(string @namespace, string key)
    {
        var entry = await FindAsync(@namespace, key);
        return entry.IsSecret ? Mask(entry) : entry;
    }

    public Task<SystemConfig> GetSecretAsync(string @namespace, string key) =>
        FindAsync(@namespace, key);

    public async Task<IEnumerable<SystemConfig>> GetAllAsync()
    {
        var all = await _crud.QueryAsync(_ => true);
        return all.Select(e => e.IsSecret ? Mask(e) : e);
    }

    public async Task<SystemConfig> UpdateAsync(string @namespace, string key, string value)
    {
        var entry = await FindAsync(@namespace, key);
        entry.Value = value;
        var updated = await _crud.UpdateAsync(entry.Id, entry)
            ?? throw new KeyNotFoundException($"System config key '{entry.Id}' was not found.");
        return updated.IsSecret ? Mask(updated) : updated;
    }

    private async Task<SystemConfig> FindAsync(string @namespace, string key)
    {
        var id = $"{@namespace}::{key}";
        return await _crud.GetByIdAsync(id)
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
            IsSecret = entry.IsSecret
        };
}
