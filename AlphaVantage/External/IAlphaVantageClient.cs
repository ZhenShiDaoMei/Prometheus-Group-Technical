using AlphaVantage.Models;

namespace AlphaVantage.External;

public interface IAlphaVantageClient
{
    Task<List<IntradayDataPoint>> GetIntradayDataAsync(string symbol, DateTime fromDate);
}
