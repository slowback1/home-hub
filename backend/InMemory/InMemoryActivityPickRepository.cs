using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace InMemory;

public class InMemoryActivityPickRepository : IActivityPickRepository
{
    private static readonly List<ActivityPick> Picks = [];

    public Task WriteAsync(ActivityPick pick)
    {
        pick.Id = Guid.NewGuid().ToString();
        Picks.Add(pick);
        return Task.CompletedTask;
    }

    public Task<ActivityPick?> GetCurrentAsync()
    {
        var current = Picks.OrderByDescending(p => p.PickedAt).FirstOrDefault();
        return Task.FromResult(current);
    }

    public Task<IEnumerable<ActivityPick>> GetRangeAsync(DateTime from, DateTime to)
    {
        var results = Picks.Where(p => p.PickedAt >= from && p.PickedAt <= to).AsEnumerable();
        return Task.FromResult(results);
    }

    public static void ClearStaticState() => Picks.Clear();
}
