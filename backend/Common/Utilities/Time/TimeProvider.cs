using System;
using Common.Interfaces;

namespace Common.Utilities.Time;

public class TimeProvider : ITimeProvider
{
    public DateTime Now()
    {
        return DateTime.Now;
    }

    public DateTime Today()
    {
        return DateTime.Today;
    }

    public DateTime FromString(string value)
    {
        return DateTime.Parse(value);
    }

    public DateTime UtcNow()
    {
        return DateTime.UtcNow;
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