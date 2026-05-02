using Common.Interfaces;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework;

public class EfActivityPickRepository(AppDbContext db) : IActivityPickRepository
{
    public async Task WriteAsync(ActivityPick pick)
    {
        pick.Id = Guid.NewGuid().ToString();
        db.ActivityPicks.Add(pick);
        await db.SaveChangesAsync();
    }

    public async Task<ActivityPick?> GetCurrentAsync() =>
        await db.ActivityPicks.OrderByDescending(p => p.PickedAt).FirstOrDefaultAsync();

    public async Task<IEnumerable<ActivityPick>> GetRangeAsync(DateTime from, DateTime to) =>
        await db.ActivityPicks
            .Where(p => p.PickedAt >= from && p.PickedAt <= to)
            .OrderBy(p => p.PickedAt)
            .ToListAsync();
}
