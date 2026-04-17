using Common.Interfaces;

namespace Common.Utilities.Time;

public static class TimeEnvironment
{
    public static ITimeProvider Provider { get; private set; } = new TimeProvider();

    public static void SetProvider(ITimeProvider provider)
    {
        Provider = provider;
    }

    public static void ResetProvider()
    {
        Provider = new TimeProvider();
    }
}