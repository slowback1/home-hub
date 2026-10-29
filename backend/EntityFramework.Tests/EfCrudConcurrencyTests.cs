using Common.Interfaces;
using Common.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFramework.Tests;

[TestFixture]
public class EfCrudConcurrencyTests
{
    [Test]
    public async Task ConcurrentCrudOperations_ThroughFactory_DoNotThrowSecondOperationError()
    {
        var connStr = $"Data Source=file:conc{Guid.NewGuid():N}?mode=memory&cache=shared";
        using var keepAlive = new SqliteConnection(connStr);
        keepAlive.Open();

        var services = new ServiceCollection();
        services.AddDbContextFactory<AppDbContext>(o => o.UseSqlite(connStr));
        services.AddScoped<ICrudFactory, EfCrudFactory>();
        using var provider = services.BuildServiceProvider();

        using (var initContext = provider.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext())
        {
            initContext.Database.EnsureCreated();
        }

        using (var seedScope = provider.CreateScope())
        {
            var seedCrud = seedScope.ServiceProvider.GetRequiredService<ICrudFactory>().GetCrud<AppUser>();
            await seedCrud.CreateAsync(new AppUser { Id = "u1", Name = "concurrency" });
        }

        using var scope = provider.CreateScope();
        var crud = scope.ServiceProvider.GetRequiredService<ICrudFactory>().GetCrud<AppUser>();

        const int workers = 16;
        var gate = new TaskCompletionSource<bool>();
        var reads = Enumerable.Range(0, workers).Select(i => Task.Run(async () =>
        {
            await gate.Task;
            return i % 2 == 0
                ? await crud.GetByIdAsync("u1")
                : await crud.GetByQueryAsync(u => u.Name == "concurrency");
        })).ToArray();
        gate.SetResult(true);

        var results = await Task.WhenAll(reads);
        Assert.That(results.All(u => u != null), Is.True);
    }
}
