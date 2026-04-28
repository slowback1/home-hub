using Common.Utilities.Time;
using TestUtilities;
using TimeProvider = Common.Utilities.Time.TimeProvider;

namespace Common.Tests.Utilities.Time;

public class TimeEnvironmentTests
{
	[TearDown]
	public void Teardown()
	{
		TimeEnvironment.ResetProvider();
	}

	[Test]
	public void HasATimeProvider()
	{
		var provider = TimeEnvironment.Provider;
		Assert.That(provider, Is.Not.Null);
	}

	[Test]
	public void DefaultTimeProviderIsTimeProvider()
	{
		var provider = TimeEnvironment.Provider;
		Assert.That(provider, Is.InstanceOf<TimeProvider>());
	}

	[Test]
	public void CanOverrideTheTimeProvider()
	{
		var provider = new TestTimeProvider(new DateTime(2022, 2, 2));
		TimeEnvironment.SetProvider(provider);
		Assert.That(TimeEnvironment.Provider, Is.SameAs(provider));
	}
}