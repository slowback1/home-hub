namespace Common.Models;

public class WeatherConditions
{
    public double Temperature { get; set; }
    public string ConditionLabel { get; set; } = string.Empty;
    public int HumidityPercent { get; set; }
    public double WindSpeed { get; set; }
    public string Units { get; set; } = string.Empty;
}
