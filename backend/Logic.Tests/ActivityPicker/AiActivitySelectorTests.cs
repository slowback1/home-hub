using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;
using Logic.ActivityPicker;
using Logic.SystemConfig;
using Microsoft.Extensions.Logging.Abstractions;
using SC = Common.Models.SystemConfig;

namespace Logic.Tests.ActivityPicker;

public class AiActivitySelectorTests
{
    private static DictionarySystemConfigProvider MakeConfig(string model = "llama3.2") =>
        new([
            new SC { Id = "activity::ai_model", Namespace = "activity", Key = "ai_model", Value = model, Type = "string" }
        ]);

    private static List<Activity> Activities(params string[] names)
    {
        var list = new List<Activity>();
        foreach (var name in names)
            list.Add(new Activity { Id = name, Name = name, Weight = 1 });
        return list;
    }

    private class StubOllamaClient(string response, bool throws = false) : IOllamaClient
    {
        public string? LastModel { get; private set; }
        public string? LastPrompt { get; private set; }

        public Task<string> ChatAsync(string model, string prompt)
        {
            LastModel = model;
            LastPrompt = prompt;
            if (throws)
                throw new Exception("Ollama unreachable");
            return Task.FromResult(response);
        }
    }

    [Test]
    public async Task SelectAsync_ReturnsActivityMatchingModelResponse()
    {
        var client = new StubOllamaClient("swim");
        var selector = new AiActivitySelector(client, MakeConfig(), new RandomActivitySelector(), NullLogger<AiActivitySelector>.Instance);
        var activities = Activities("swim", "run", "read");

        var result = await selector.SelectAsync(activities, []);

        Assert.That(result.Name, Is.EqualTo("swim"));
    }

    [Test]
    public async Task SelectAsync_MatchesResponseCaseInsensitively()
    {
        var client = new StubOllamaClient("  SWIM  ");
        var selector = new AiActivitySelector(client, MakeConfig(), new RandomActivitySelector(), NullLogger<AiActivitySelector>.Instance);
        var activities = Activities("swim", "run");

        var result = await selector.SelectAsync(activities, []);

        Assert.That(result.Name, Is.EqualTo("swim"));
    }

    [Test]
    public async Task SelectAsync_FallsBackToRandom_WhenResponseDoesNotMatchAnyActivity()
    {
        var client = new StubOllamaClient("sleep");
        var selector = new AiActivitySelector(client, MakeConfig(), new RandomActivitySelector(), NullLogger<AiActivitySelector>.Instance);
        var activities = Activities("swim", "run");

        var result = await selector.SelectAsync(activities, []);

        Assert.That(new[] { "swim", "run" }, Contains.Item(result.Name));
    }

    [Test]
    public async Task SelectAsync_FallsBackToRandom_WhenOllamaThrows()
    {
        var client = new StubOllamaClient("swim", throws: true);
        var selector = new AiActivitySelector(client, MakeConfig(), new RandomActivitySelector(), NullLogger<AiActivitySelector>.Instance);
        var activities = Activities("swim", "run");

        var result = await selector.SelectAsync(activities, []);

        Assert.That(new[] { "swim", "run" }, Contains.Item(result.Name));
    }

    [Test]
    public async Task SelectAsync_SendsModelFromConfig()
    {
        var client = new StubOllamaClient("swim");
        var selector = new AiActivitySelector(client, MakeConfig("mistral"), new RandomActivitySelector(), NullLogger<AiActivitySelector>.Instance);
        var activities = Activities("swim");

        await selector.SelectAsync(activities, []);

        Assert.That(client.LastModel, Is.EqualTo("mistral"));
    }

    [Test]
    public async Task SelectAsync_IncludesAllActivityNamesInPrompt()
    {
        var client = new StubOllamaClient("swim");
        var selector = new AiActivitySelector(client, MakeConfig(), new RandomActivitySelector(), NullLogger<AiActivitySelector>.Instance);
        var activities = Activities("swim", "run", "read");

        await selector.SelectAsync(activities, []);

        Assert.That(client.LastPrompt, Does.Contain("swim"));
        Assert.That(client.LastPrompt, Does.Contain("run"));
        Assert.That(client.LastPrompt, Does.Contain("read"));
    }

    [Test]
    public async Task SelectAsync_IncludesRecentPickNamesInPrompt()
    {
        var client = new StubOllamaClient("swim");
        var selector = new AiActivitySelector(client, MakeConfig(), new RandomActivitySelector(), NullLogger<AiActivitySelector>.Instance);
        var activities = Activities("swim", "run");
        var recentPicks = new List<ActivityPick>
        {
            new() { ActivityName = "run" },
            new() { ActivityName = "swim" }
        };

        await selector.SelectAsync(activities, recentPicks);

        Assert.That(client.LastPrompt, Does.Contain("run"));
        Assert.That(client.LastPrompt, Does.Contain("swim"));
    }
}
