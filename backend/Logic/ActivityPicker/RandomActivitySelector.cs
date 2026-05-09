using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.ActivityPicker;

public class RandomActivitySelector : IActivitySelector
{
    public Task<Activity> SelectAsync(IList<Activity> activities, IList<ActivityPick> recentPicks)
    {
        return Task.FromResult(WeightedRandom(activities));
    }

    private static Activity WeightedRandom(IList<Activity> activities)
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
