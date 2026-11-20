using System;
using System.Threading.Tasks;
using Common.Interfaces;

namespace Logic.NbaSchedule;

public class NbaScheduleFetchJob(INbaScheduleProvider provider, INbaScheduleRepository repository)
{
    public async Task ExecuteAsync()
    {
        var from = DateTime.UtcNow.Date;
        var to = from.AddDays(3);

        try
        {
            var games = await provider.GetScheduleAsync(from, to);
            await repository.UpsertAsync(games);
            await repository.PruneBeforeAsync(DateTime.UtcNow.AddDays(-3));
        }
        catch (Exception)
        {
            // Provider failure: keep the previous cache intact (dashboard degrades gracefully).
        }
    }
}
