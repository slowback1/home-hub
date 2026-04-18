using Microsoft.EntityFrameworkCore;
using Common.Models;

namespace EntityFramework;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<ExampleData> ExampleData => Set<ExampleData>();
}
