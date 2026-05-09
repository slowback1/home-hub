using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Interfaces;

public interface IActivitySelector
{
    Task<Activity> SelectAsync(IList<Activity> activities, IList<ActivityPick> recentPicks);
}
