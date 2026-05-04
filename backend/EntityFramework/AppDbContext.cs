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
    public DbSet<SystemConfig> SystemConfigs => Set<SystemConfig>();
    public DbSet<SystemConfigOption> SystemConfigOptions => Set<SystemConfigOption>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<ActivityPick> ActivityPicks => Set<ActivityPick>();
    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<FeatureFlag>().HasKey(f => f.Name);
        modelBuilder.Entity<SystemConfigOption>().HasKey(o => new { o.SystemConfigId, o.Value });
        modelBuilder.Entity<SystemConfigOption>()
            .HasOne(o => o.SystemConfig)
            .WithMany(c => c.Options)
            .HasForeignKey(o => o.SystemConfigId);
    }
}
