using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.ActivityPicker;

public class ActivityPickerJob(ICrudFactory crudFactory, IActivityPickRepository pickRepository, IActivitySelector selector)
{
    private readonly ICrud<Activity> _activities = crudFactory.GetCrud<Activity>();

    public async Task ExecuteAsync()
    {
        var activities = (await _activities.QueryAsync(_ => true)).ToList();
        if (activities.Count == 0)
            return;

        var recentPicks = (await pickRepository.GetRecentAsync(activities.Count * 2)).ToList();
        var selected = await selector.SelectAsync(activities, recentPicks);
        await pickRepository.WriteAsync(new ActivityPick
        {
            ActivityName = selected.Name,
            PickedAt = DateTime.UtcNow
        });
    }
}
