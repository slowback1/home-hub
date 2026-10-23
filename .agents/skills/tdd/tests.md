# Good and Bad Tests

## Good Tests

**Integration-style**: Test through real interfaces, not mocks of internal parts.

```csharp
// GOOD: Tests observable behavior
[Test]
public void PlayerCanPickUpHealthPotion()
{
    var player = new Player();
    var potion = new HealthPotion(heal: 20);
    player.Inventory.Add(potion);

    player.UseItem(potion);

    Assert.That(player.Health, Is.EqualTo(player.MaxHealth));
}
```

Characteristics:

- Tests behavior players/callers care about
- Uses public API only
- Survives internal refactors
- Describes WHAT, not HOW
- One logical assertion per test

## Bad Tests

**Implementation-detail tests**: Coupled to internal structure.

```csharp
// BAD: Tests implementation details
[Test]
public void UseItemCallsHealMethod()
{
    var mockHealer = Substitute.For<IHealer>();
    var player = new Player(mockHealer);
    player.UseItem(new HealthPotion(heal: 20));
    mockHealer.Received().Heal(20); // asserting on call, not on outcome
}
```

Red flags:

- Mocking internal collaborators
- Testing private methods
- Asserting on call counts/order
- Test breaks when refactoring without behavior change
- Test name describes HOW not WHAT

```csharp
// BAD: Bypasses interface to verify
[Test]
public void SaveGameWritesToDisk()
{
    var save = new SaveSystem();
    save.Save(gameState);
    Assert.That(File.Exists("save.dat")); // reaches past the interface
}

// GOOD: Verifies through interface
[Test]
public void SavedGameCanBeLoaded()
{
    var save = new SaveSystem();
    save.Save(gameState);
    var loaded = save.Load();
    Assert.That(loaded.PlayerLevel, Is.EqualTo(gameState.PlayerLevel));
}
```
