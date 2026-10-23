# When to Mock

Mock at **system boundaries** only:

- External I/O (databases, network, platform APIs)
- Time/randomness (e.g. `IRandom`, `IClock`)
- Platform services (OS, analytics, notifications)

Don't mock:

- Your own domain logic classes
- Internal collaborators (e.g. `ActivityService` talking to `ActivityRepository`)
- Anything you control

## Designing for Mockability

At system boundaries, design interfaces that are easy to mock:

**1. Use dependency injection**

Pass external dependencies in rather than creating them internally:

```csharp
// Easy to mock
public class WheelPicker
{
    private readonly IRandom _rng;
    public WheelPicker(IRandom rng) { _rng = rng; }
    public Activity Pick(IReadOnlyList<Activity> activities) => activities[_rng.Next(activities.Count)];
}

// Hard to mock
public class WheelPicker
{
    public Activity Pick(IReadOnlyList<Activity> activities) => activities[new Random().Next(activities.Count)]; // creates its own dependency, untestable
}
```

**2. Prefer specific interfaces over generic ones**

Create specific methods for each external operation:

```csharp
// GOOD: Each operation is independently mockable
public interface IAudiobookJobRepository
{
    void Save(AudiobookJob job);
    AudiobookJob Get(Guid id);
    bool Exists(Guid id);
}

// BAD: One generic method forces conditional logic in test setup
public interface IStorage
{
    object Execute(string operation, object payload);
}
```

The specific interface approach means:
- Each mock returns one clear shape
- No conditional logic in test setup
- Easier to see which operations a test exercises
- Strong typing per operation
