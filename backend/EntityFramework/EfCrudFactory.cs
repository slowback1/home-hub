using Common.Interfaces;

namespace EntityFramework;

public class EfCrudFactory : ICrudFactory
{
    private readonly AppDbContext _context;
    public EfCrudFactory(AppDbContext context)
    {
        _context = context;
    }
    public ICrud<T> GetCrud<T>() where T : class, IIdentifyable
        => new EfCrud<T>(_context);
}
