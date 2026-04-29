using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Interfaces;
using SC = Common.Models.SystemConfig;

namespace Logic.SystemConfig;

public class DictionarySystemConfigProvider(IEnumerable<SC> entries) : ISystemConfigProvider
{
    private readonly List<SC> _entries = entries.ToList();

    public Task<SC> GetAsync(string @namespace, string key)
    {
        var entry = Find(@namespace, key);
        return Task.FromResult(entry.IsSecret ? Mask(entry) : entry);
    }

    public Task<SC> GetSecretAsync(string @namespace, string key)
    {
        return Task.FromResult(Find(@namespace, key));
    }

    public Task<IEnumerable<SC>> GetAllAsync()
    {
        var results = _entries.Select(e => e.IsSecret ? Mask(e) : e);
        return Task.FromResult(results);
    }

    public Task<SC> UpdateAsync(string @namespace, string key, string value)
    {
        var entry = Find(@namespace, key);
        entry.Value = value;
        return Task.FromResult(entry.IsSecret ? Mask(entry) : entry);
    }

    private SC Find(string @namespace, string key)
    {
        var id = $"{@namespace}::{key}";
        return _entries.FirstOrDefault(e => e.Id == id)
            ?? throw new KeyNotFoundException($"System config key '{id}' was not found.");
    }

    private static SC Mask(SC entry) =>
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
