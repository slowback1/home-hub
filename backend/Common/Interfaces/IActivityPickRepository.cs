using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Interfaces;

public interface IActivityPickRepository
{
    Task WriteAsync(ActivityPick pick);
    Task<ActivityPick?> GetCurrentAsync();
    Task<IEnumerable<ActivityPick>> GetRangeAsync(DateTime from, DateTime to);
    Task ClearAllAsync();
}
