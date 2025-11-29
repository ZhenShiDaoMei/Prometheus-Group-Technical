using AlphaVantage.External;
using AlphaVantage.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IAlphaVantageClient, AlphaVantageClient>();
builder.Services.AddScoped<IIntradayService, IntradayService>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
