using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.ActivityPicker;

public class ActivityPickerJob(ICrudFactory crudFactory, IActivityPickRepository pickRepository, IActivitySelector selector)
{
    private readonly ICrud<Activity> _activities = crudFactory.GetCrud<Activity>();
    private readonly ICrud<AppUser> _users = crudFactory.GetCrud<AppUser>();

    public async Task ExecuteAsync()
    {
        var users = await _users.QueryAsync(_ => true);
        foreach (var user in users)
        {
            var activities = (await _activities.QueryAsync(a => a.UserId == user.Id)).ToList();
            if (activities.Count == 0)
                continue;

            var recentPicks = (await pickRepository.GetRecentAsync(user.Id, activities.Count * 2)).ToList();
            var selected = await selector.SelectAsync(activities, recentPicks);
            await pickRepository.WriteAsync(new ActivityPick
            {
                UserId = user.Id,
                ActivityName = selected.Name,
                PickedAt = DateTime.UtcNow
            });
        }
    }
}
