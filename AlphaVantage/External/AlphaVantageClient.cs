using AlphaVantage.Models;
using System.Text.Json;

namespace AlphaVantage.External;

public class AlphaVantageClient : IAlphaVantageClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public AlphaVantageClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["AlphaVantage:ApiKey"] ?? throw new ArgumentNullException("Missing AlphaVantage API key");
    }

    public async Task<List<IntradayDataPoint>> GetIntradayDataAsync(string symbol, DateTime fromDate)
    {
        // This was my understanding for getting intraday data for last month:
        // - Direct previous month, ex: if ran in November, we get data from October. (If you meant last 30 days we just need to remove "month={year}-{month}" from queryURL)
        
        // In this case I will be using this example given by the alphavantage doc:
        // https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol=IBM&interval=5min&month=2009-01&outputsize=full&apikey=demo

        string month = fromDate.ToString("MM");
        string year = fromDate.ToString("yyyy");
        // using 'outputsize=compact' because I don't have premium API. Proper solution should be 'outputsize=full'
        string queryURL = $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={symbol}&interval=15min&month={year}-{month}&outputsize=compact&apikey={_apiKey}";

        Uri queryUri = new Uri(queryURL); 
        HttpResponseMessage response = await _httpClient.GetAsync(queryUri);
        response.EnsureSuccessStatusCode(); 

        string intradayJSON = await response.Content.ReadAsStringAsync();
        // Console.WriteLine("API Response: " + intradayJSON);
        List<IntradayDataPoint> dataPoints = [];
        using JsonDocument doc = JsonDocument.Parse(intradayJSON);
        JsonElement root = doc.RootElement;

        if (root.TryGetProperty("Time Series (15min)", out JsonElement timeSeries))
        {
            foreach (JsonProperty timestamp in timeSeries.EnumerateObject())
            {
                string timestampString = timestamp.Name;
                JsonElement timestampData = timestamp.Value;

                DateTime time = DateTime.ParseExact(timestamp.Name, "yyyy-MM-dd HH:mm:ss", null);
                decimal open = decimal.Parse(timestampData.GetProperty("1. open").GetString() ?? "0");
                decimal high = decimal.Parse(timestampData.GetProperty("2. high").GetString() ?? "0");
                decimal low = decimal.Parse(timestampData.GetProperty("3. low").GetString() ?? "0");
                decimal close = decimal.Parse(timestampData.GetProperty("4. close").GetString() ?? "0");
                long volume = long.Parse(timestampData.GetProperty("5. volume").GetString() ?? "0");

                IntradayDataPoint dataPoint = new IntradayDataPoint
                {
                    Time = time,
                    Open = open,
                    High = high,
                    Low = low,
                    Close = close,
                    Volume = volume
                };

                dataPoints.Add(dataPoint);
            }
        } else
        {
            Console.WriteLine($"Alpha Vantage API Response: {intradayJSON}");
            throw new InvalidOperationException($"No intraday data found last month for symbol '{symbol}'.");
        }

        return dataPoints;
    }
}
