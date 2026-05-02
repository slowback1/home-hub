using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.ActivityPicker;

public class ActivityPickerJob(ICrudFactory crudFactory, IActivityPickRepository pickRepository)
{
    private readonly ICrud<Activity> _activities = crudFactory.GetCrud<Activity>();

    public async Task ExecuteAsync()
    {
        var activities = (await _activities.QueryAsync(_ => true)).ToList();
        if (activities.Count == 0)
            return;

        var selected = WeightedRandom(activities);
        await pickRepository.WriteAsync(new ActivityPick
        {
            ActivityName = selected.Name,
            PickedAt = DateTime.UtcNow
        });
    }

    private static Activity WeightedRandom(System.Collections.Generic.List<Activity> activities)
    {
        var totalWeight = activities.Sum(a => a.Weight);
        var roll = new Random().Next(totalWeight);
        var cumulative = 0;
        foreach (var activity in activities)
        {
            cumulative += activity.Weight;
            if (roll < cumulative)
                return activity;
        }
        return activities[^1];
    }
}
