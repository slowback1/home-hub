using System;
using Common.Interfaces;

namespace TestUtilities;

public class TestTimeProvider(DateTime now) : ITimeProvider
{
	public DateTime Now()
	{
		return now;
	}

	public DateTime Today()
	{
		return now.Date;
	}

	public DateTime FromString(string value)
	{
		return DateTime.Parse(value);
	}

	public DateTime UtcNow()
	{
		return now.ToUniversalTime();
	}

	public DateTime AddDays(DateTime date, int days)
	{
		return date.AddDays(days);
	}

	public DateTime AddHours(DateTime date, int hours)
	{
		return date.AddHours(hours);
	}

	public DateTime AddMinutes(DateTime date, int minutes)
	{
		return date.AddMinutes(minutes);
	}

	public DateTime AddSeconds(DateTime date, int seconds)
	{
		return date.AddSeconds(seconds);
	}
}