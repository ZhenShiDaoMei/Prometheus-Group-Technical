using AlphaVantage.External;
using AlphaVantage.Models;

namespace AlphaVantage.Services;

public class IntradayService : IIntradayService
{
    private readonly IAlphaVantageClient _alphaVantageClient;

    public IntradayService(IAlphaVantageClient alphaVantageClient)
    {
        _alphaVantageClient = alphaVantageClient;
    }

    public async Task<List<DailyStats>> GetDailyStatsAsync(string symbol)
    {
        List<IntradayDataPoint> dataPoints = await _alphaVantageClient.GetIntradayDataAsync(symbol, DateTime.UtcNow.AddMonths(-1));

        var dailyData = dataPoints
        .GroupBy(x => x.Time.Date) // Group by same day
        .Select(group => new DailyStats // Get averages and total volume
        {
            Day = group.Key.ToString("yyyy-MM-dd HH:mm:ss"),
            LowAverage = group.Average(x => x.Low),
            HighAverage = group.Average(x => x.High),
            Volume = group.Sum(x => x.Volume)
        })
        .ToList();
        
        return dailyData;
    }
}