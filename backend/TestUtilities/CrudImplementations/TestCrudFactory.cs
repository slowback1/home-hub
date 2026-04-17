using System;
using System.Collections.Concurrent;
using Common.Interfaces;

namespace TestUtilities.CrudImplementations;

public class TestCrudFactory : ICrudFactory
{
    private readonly ConcurrentDictionary<Type, dynamic> _crudInstances = new();

    public ICrud<T> GetCrud<T>() where T : class, IIdentifyable
    {
        if (_crudInstances.ContainsKey(typeof(T))) return (ICrud<T>)_crudInstances[typeof(T)];
        ICrud<T> crudInstance = new TestGenericCrud<T>();
        _crudInstances.TryAdd(typeof(T), crudInstance);
        return crudInstance;
    }
}