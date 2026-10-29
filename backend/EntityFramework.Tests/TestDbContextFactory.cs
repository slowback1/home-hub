using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Tests;

public class TestDbContextFactory(DbContextOptions<AppDbContext> options, bool includeTestEntity = false) : IDbContextFactory<AppDbContext>, IDisposable
{
    public AppDbContext CreateDbContext() =>
        includeTestEntity ? new TestAppDbContext(options) : new AppDbContext(options);

    public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(CreateDbContext());

    public void Dispose()
    {
    }
}
