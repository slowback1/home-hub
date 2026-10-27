using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Interfaces;

public interface IActivityPickRepository
{
    Task WriteAsync(ActivityPick pick);
    Task<ActivityPick?> GetCurrentAsync(string userId);
    Task<IEnumerable<ActivityPick>> GetRangeAsync(string userId, DateTime from, DateTime to);
    Task<IEnumerable<ActivityPick>> GetRecentAsync(string userId, int count);
    Task ClearAllAsync();
}
