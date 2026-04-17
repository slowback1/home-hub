using System;
using System.IO;
using Common.Interfaces;
using Common.Utilities.Time;

namespace Common.Utilities.Logging.LoggingEngines;

public class FileLoggingEngineSettings
{
    public FileLoggingTimestampFormat TimestampFormat { get; set; }
    public FileLogRotationStrategy LogRotationStrategy { get; set; }
}

public enum FileLoggingTimestampFormat
{
    None,
    Short,
    Long
}

public enum FileLogRotationStrategy
{
    Daily,
    Weekly,
    Monthly
}

public class FileLoggingEngine : ILoggingEngine
{
    private static readonly FileLoggingEngineSettings DefaultSettings = new()
    {
        TimestampFormat = FileLoggingTimestampFormat.None,
        LogRotationStrategy = FileLogRotationStrategy.Daily
    };

    private readonly ITimeProvider _timeProvider;

    public FileLoggingEngine()
    {
        _timeProvider = TimeEnvironment.Provider;
        Settings = DefaultSettings;
    }

    public FileLoggingEngine(FileLoggingEngineSettings settings)
    {
        _timeProvider = TimeEnvironment.Provider;
        Settings = settings;
    }

    private FileLoggingEngineSettings Settings { get; }

    public void Log(string message)
    {
        if (!FileExists())
            CreateFile();
        AppendToFile(message);
    }

    private bool FileExists()
    {
        var filePath = Path.Combine(Path.GetTempPath(), GetFileName());
        return File.Exists(filePath);
    }

    private void CreateFile()
    {
        var filePath = Path.Combine(Path.GetTempPath(), GetFileName());
        File.WriteAllText(filePath, string.Empty);
    }

    private void AppendToFile(string message)
    {
        var filePath = Path.Combine(Path.GetTempPath(), GetFileName());
        File.AppendAllText(filePath, GetLogMessage(message) + Environment.NewLine);
    }

    private string GetLogMessage(string baseMessage)
    {
        return $"{GetTimestamp()}{baseMessage}";
    }

    private string GetFileName()
    {
        return $"{GetLogDate():yyyy-MM-dd}.log";
    }

    private DateTime GetLogDate()
    {
        switch (Settings.LogRotationStrategy)
        {
            case FileLogRotationStrategy.Daily:
                return GetToday();
            case FileLogRotationStrategy.Weekly:
                return GetTheMostRecentMonday();
            case FileLogRotationStrategy.Monthly:
                return GetFirstDayOfTheMonth();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private DateTime GetFirstDayOfTheMonth()
    {
        var today = _timeProvider.Today();
        return new DateTime(today.Year, today.Month, 1);
    }

    private DateTime GetTheMostRecentMonday()
    {
        var today = _timeProvider.Today();
        var daysUntilMonday = (int)today.DayOfWeek - 1;
        if (daysUntilMonday < 0) daysUntilMonday = 6;
        return _timeProvider.AddDays(today, -daysUntilMonday);
    }

    private DateTime GetToday()
    {
        return _timeProvider.Today();
    }

    private string GetTimestamp()
    {
        return Settings.TimestampFormat switch
        {
            FileLoggingTimestampFormat.None => string.Empty,
            FileLoggingTimestampFormat.Short => $"({DateTime.Now:HH:mm:ss}) ",
            FileLoggingTimestampFormat.Long => $"({DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}) ",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}