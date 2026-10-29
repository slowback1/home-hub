using System.Linq.Expressions;
using Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework;

public class EfCrud<T>(IDbContextFactory<AppDbContext> contextFactory) : ICrud<T> where T : class, IIdentifyable
{
    public async Task<T> CreateAsync(T item)
    {
        if (string.IsNullOrWhiteSpace(item.Id))
        {
            item.Id = Guid.NewGuid().ToString();
        }
        await using var context = await contextFactory.CreateDbContextAsync();
        await context.Set<T>().AddAsync(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<T?> UpdateAsync(string id, T item)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        var existing = await context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
        if (existing == null)
            return null;
        context.Entry(existing).CurrentValues.SetValues(item);
        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        var entity = await context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return false;
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<T?> GetByQueryAsync(Expression<Func<T, bool>> query)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Set<T>().FirstOrDefaultAsync(query);
    }

    public async Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> query)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Set<T>().Where(query).ToListAsync();
    }
}
