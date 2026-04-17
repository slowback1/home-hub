using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Interfaces;

namespace TestUtilities.CrudImplementations;

public abstract class TestCrud<T> : ICrud<T> where T : class, IIdentifyable
{
    private readonly List<T> _items = new();
    private int _nextId = 1;

    public virtual Task<T> CreateAsync(T item)
    {
        item.Id = (_nextId++).ToString();

        _items.Add(item);
        return Task.FromResult(item);
    }

    public Task<T?> GetByIdAsync(string id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        return Task.FromResult<T?>(item);
    }

    public Task<T?> UpdateAsync(string id, T item)
    {
        var existingItem = _items.FirstOrDefault(i => i.Id == id);
        if (existingItem == null) return Task.FromResult<T?>(null);

        var index = _items.IndexOf(existingItem);
        if (index < 0) return Task.FromResult<T?>(null);

        _items[index] = item;
        return Task.FromResult(existingItem)!;
    }

    public Task<bool> DeleteAsync(string id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item == null) return Task.FromResult(false);

        _items.Remove(item);
        return Task.FromResult(true);
    }

    public Task<T?> GetByQueryAsync(Func<T, bool> query)
    {
        var item = _items.FirstOrDefault(query);
        return Task.FromResult<T?>(item);
    }

    public Task<IEnumerable<T>> QueryAsync(Func<T, bool> query)
    {
        var results = _items.Where(query).ToList();
        return Task.FromResult(results.AsEnumerable());
    }
}