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
    public DbSet<ComfyUiWorkflow> ComfyUiWorkflows => Set<ComfyUiWorkflow>();
    public DbSet<ChoreTask> ChoreTasks => Set<ChoreTask>();
    public DbSet<TaskCompletion> TaskCompletions => Set<TaskCompletion>();
    public DbSet<Bookmark> Bookmarks => Set<Bookmark>();
    public DbSet<DashboardSlot> DashboardSlots => Set<DashboardSlot>();
    public DbSet<WalkSession> WalkSessions => Set<WalkSession>();
    public DbSet<Wheel> Wheels => Set<Wheel>();
    public DbSet<ShoppingList> ShoppingLists => Set<ShoppingList>();
    public DbSet<ShoppingListItem> ShoppingListItems => Set<ShoppingListItem>();
    public DbSet<SavedItem> SavedItems => Set<SavedItem>();
    public DbSet<PremadeList> PremadeLists => Set<PremadeList>();
    public DbSet<PremadeListItem> PremadeListItems => Set<PremadeListItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<FeatureFlag>().HasKey(f => f.Name);
        modelBuilder.Entity<SystemConfigOption>().HasKey(o => new { o.SystemConfigId, o.Value });
        modelBuilder.Entity<SystemConfigOption>()
            .HasOne<SystemConfig>()
            .WithMany(c => c.Options)
            .HasForeignKey(o => o.SystemConfigId);
        modelBuilder.Entity<TaskCompletion>()
            .HasOne<ChoreTask>()
            .WithMany()
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Activity>()
            .HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(a => a.UserId);
        modelBuilder.Entity<ActivityPick>()
            .HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(p => p.UserId);
        modelBuilder.Entity<Bookmark>()
            .HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(b => b.UserId);
        modelBuilder.Entity<Wheel>()
            .HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(w => w.UserId);
        modelBuilder.Entity<ShoppingList>()
            .HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(l => l.UserId);
        modelBuilder.Entity<ShoppingListItem>()
            .HasOne<ShoppingList>()
            .WithMany()
            .HasForeignKey(i => i.ListId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<SavedItem>()
            .HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(s => s.UserId);
        modelBuilder.Entity<PremadeList>()
            .HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(l => l.UserId);
        modelBuilder.Entity<PremadeListItem>()
            .HasOne<PremadeList>()
            .WithMany()
            .HasForeignKey(i => i.PremadeListId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChoreTask>()
            .HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(t => t.UserId);

        modelBuilder.Entity<DashboardSlot>()
            .HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(s => s.UserId);

        modelBuilder.Entity<DashboardSlot>()
            .HasIndex(s => new { s.UserId, s.LayoutFormat, s.SlotIndex })
            .IsUnique();
    }
}
