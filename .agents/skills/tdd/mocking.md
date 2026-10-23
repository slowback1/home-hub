# When to Mock

Mock at **system boundaries** only:

- External I/O (save files, network, platform APIs)
- Time/randomness (e.g. `IRandom`, `IClock`)
- Platform services (OS, analytics, achievements)

Don't mock:

- Your own game logic classes
- Internal collaborators (e.g. `InventorySystem` talking to `ItemDatabase`)
- Anything you control

## Designing for Mockability

At system boundaries, design interfaces that are easy to mock:

**1. Use dependency injection**

Pass external dependencies in rather than creating them internally:

```csharp
// Easy to mock
public class LootTable
{
    private readonly IRandom _rng;
    public LootTable(IRandom rng) { _rng = rng; }
    public Item Roll() => _items[_rng.Next(_items.Count)];
}

// Hard to mock
public class LootTable
{
    public Item Roll() => _items[GD.Randi() % _items.Count]; // static global, untestable
}
```

**2. Prefer specific interfaces over generic ones**

Create specific methods for each external operation:

```csharp
// GOOD: Each operation is independently mockable
public interface ISaveSystem
{
    void Save(GameState state);
    GameState Load();
    bool HasSave();
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
