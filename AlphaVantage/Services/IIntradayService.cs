using AlphaVantage.Models;

namespace AlphaVantage.Services;

public interface IIntradayService
{
    Task<List<DailyStats>> GetDailyStatsAsync(string symbol);
}
