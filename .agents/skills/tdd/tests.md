# Good and Bad Tests

## Good Tests

**Integration-style**: Test through real interfaces, not mocks of internal parts.

```csharp
// GOOD: Tests observable behavior
[Test]
public void PickedActivityIsAddedToSessionHistory()
{
    var picker = new ActivityPicker();
    var activity = new Activity("Walk");
    picker.AddActivity(activity);

    picker.Pick(activity);

    Assert.That(picker.History, Contains.Item(activity));
}
```

Characteristics:

- Tests behavior users/callers care about
- Uses public API only
- Survives internal refactors
- Describes WHAT, not HOW
- One logical assertion per test

## Bad Tests

**Implementation-detail tests**: Coupled to internal structure.

```csharp
// BAD: Tests implementation details
[Test]
public void PickCallsHistoryRecorderMethod()
{
    var mockRecorder = Substitute.For<IHistoryRecorder>();
    var picker = new ActivityPicker(mockRecorder);
    picker.Pick(new Activity("Walk"));
    mockRecorder.Received().Record(Arg.Any<Activity>()); // asserting on call, not on outcome
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
public void AudiobookConversionWritesFileToDisk()
{
    var converter = new AudiobookConverter();
    converter.Convert(epub);
    Assert.That(File.Exists("output.m4b")); // reaches past the interface
}

// GOOD: Verifies through interface
[Test]
public void ConvertedAudiobookIsRetrievableFromTheJob()
{
    var converter = new AudiobookConverter();
    var job = converter.Convert(epub);
    var stored = converter.GetJob(job.Id);
    Assert.That(stored.OutputFormat, Is.EqualTo("m4b"));
}
```
