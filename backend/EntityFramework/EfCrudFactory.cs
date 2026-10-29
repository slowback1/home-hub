using Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework;

public class EfCrudFactory(IDbContextFactory<AppDbContext> contextFactory) : ICrudFactory
{
    public ICrud<T> GetCrud<T>() where T : class, IIdentifyable
        => new EfCrud<T>(contextFactory);
}
