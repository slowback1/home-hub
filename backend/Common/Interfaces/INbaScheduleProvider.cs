using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Interfaces;

public interface INbaScheduleProvider
{
    Task<IEnumerable<NbaScheduledGame>> GetScheduleAsync(DateTime from, DateTime to);
}
