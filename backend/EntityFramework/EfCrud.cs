using System.Linq.Expressions;
using Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework;

public class EfCrud<T> : ICrud<T> where T : class, IIdentifyable
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public EfCrud(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T> CreateAsync(T item)
    {
        if (string.IsNullOrWhiteSpace(item.Id))
        {
            item.Id = Guid.NewGuid().ToString();
        }
        await _dbSet.AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public Task<T?> GetByIdAsync(string id)
        => _dbSet.FirstOrDefaultAsync(x => x.Id == id)!;

    public async Task<T?> UpdateAsync(string id, T item)
    {
        var existing = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
        if (existing == null)
            return null;
        _context.Entry(existing).CurrentValues.SetValues(item);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return false;
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<T?> GetByQueryAsync(Expression<Func<T, bool>> query)
        => _dbSet.FirstOrDefaultAsync(query)!;

    public async Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> query)
        => await _dbSet.Where(query).ToListAsync();
}