using AlphaVantage.Models;
using AlphaVantage.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlphaVantage.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly IIntradayService _intradayService;

    public StatsController(IIntradayService intradayService)
    {
        _intradayService = intradayService;
    }

    [HttpGet("intraday")]
    public async Task<ActionResult<List<DailyStats>>> GetIntradayStats(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            return BadRequest("Symbol parameter cannot be empty.");
        }

        symbol = symbol.ToUpper();
        
        try 
        {
            var stats = await _intradayService.GetDailyStatsAsync(symbol);
            return Ok(stats);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        
    }
}
