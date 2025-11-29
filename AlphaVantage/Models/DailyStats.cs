namespace AlphaVantage.Models;

public class DailyStats
{
    public required string Day { get; set; }
    public decimal LowAverage { get; set; }
    public decimal HighAverage { get; set; }
    public long Volume { get; set; }
}
