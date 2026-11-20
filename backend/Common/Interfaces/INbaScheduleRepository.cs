using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Interfaces;

public interface INbaScheduleRepository
{
    Task UpsertAsync(IEnumerable<NbaScheduledGame> games);
    Task PruneBeforeAsync(DateTime cutoff);
    Task<IEnumerable<NbaGame>> GetRangeAsync(DateTime from, DateTime to);
    Task ClearAllAsync();
}
