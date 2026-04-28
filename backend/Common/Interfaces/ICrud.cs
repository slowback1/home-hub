using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Common.Interfaces;

public interface ICrud<T> where T : class, IIdentifyable
{
    Task<T> CreateAsync(T item);
    Task<T?> GetByIdAsync(string id);
    Task<T?> UpdateAsync(string id, T item);
    Task<bool> DeleteAsync(string id);
    Task<T?> GetByQueryAsync(Expression<Func<T, bool>> query);
    Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> query);
}