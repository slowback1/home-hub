using TestUtilities;

namespace Common.Tests.Utilities;

public class DummyClass
{
	public int? Value { get; set; }
	public string? Name { get; set; } = string.Empty;
}

public class ObjectTestingUtilitiesTests
{
	[Test]
	public void CanAssertObjectHasNoNulls()
	{
		var obj = new DummyClass
		{
			Value = 5,
			Name = "Test"
		};

		obj.HasNoNulls();
	}
}