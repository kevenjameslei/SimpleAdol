using Microsoft.AspNetCore.Mvc;
using StockAnalysiserAPI.Services;
using System.Runtime.InteropServices;

namespace StockAnalysiserAPI.Controllers
{
    [ApiController]
    [Route("api/kline")]
    public class KLineController : ControllerBase
    {
        private readonly IKBarService _service;

        public KLineController(IKBarService service)
        {
            _service = service;
        }

        [HttpGet("{symbol}")]
        public async Task<IActionResult> GetKLines(string symbol, [FromQuery] string period = "1d", [FromQuery] DateTime? start = null, [FromQuery] DateTime? end = null, [FromQuery] int limit = 500)
        {
            var data = await _service.GetKLinesAsync(symbol, period, start, end, limit);
            return Ok(data);
        }
    }
}