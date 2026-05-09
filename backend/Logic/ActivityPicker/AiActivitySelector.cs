using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;
using Microsoft.Extensions.Logging;

namespace Logic.ActivityPicker;

public class AiActivitySelector(
    IOllamaClient ollamaClient,
    ISystemConfigProvider configProvider,
    RandomActivitySelector fallback,
    ILogger<AiActivitySelector> logger) : IActivitySelector
{
    public async Task<Activity> SelectAsync(IList<Activity> activities, IList<ActivityPick> recentPicks)
    {
        try
        {
            var model = (await configProvider.GetAsync("activity", "ai_model")).Value;
            var prompt = BuildPrompt(activities, recentPicks);
            var response = await ollamaClient.ChatAsync(model, prompt);
            var matched = MatchActivity(activities, response);
            if (matched is not null)
                return matched;

            logger.LogWarning("AiActivitySelector: model response '{Response}' did not match any activity; falling back to random", response.Trim());
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "AiActivitySelector: failed to get AI selection; falling back to random");
        }

        return await fallback.SelectAsync(activities, recentPicks);
    }

    private static string BuildPrompt(IList<Activity> activities, IList<ActivityPick> recentPicks)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are helping choose an activity. Here is the list of available activities:");
        foreach (var a in activities)
            sb.AppendLine($"- {a.Name}");

        if (recentPicks.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Recent picks (most recent first) — try to choose something different:");
            foreach (var p in recentPicks)
                sb.AppendLine($"- {p.ActivityName}");
        }

        sb.AppendLine();
        sb.AppendLine("Reply with exactly one activity name from the list above and nothing else.");
        return sb.ToString();
    }

    private static Activity? MatchActivity(IList<Activity> activities, string response)
    {
        var trimmed = response.Trim();
        return activities.FirstOrDefault(a =>
            string.Equals(a.Name, trimmed, StringComparison.OrdinalIgnoreCase));
    }
}
