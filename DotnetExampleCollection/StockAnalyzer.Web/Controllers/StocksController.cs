using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using StockAnalyzer.Core;
using StockAnalyzer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StockAnalyzer.Web.Controllers
{
    [ApiController]
    [Route("api/stocks")]
    public class StocksController : ControllerBase
    {
       
        [HttpGet("{ticker}")]
        public async Task<ActionResult<StockPrice>> Get(string ticker,CancellationToken token)
        {
            var store = new DataStore();
            var result  = await store.LoadStocks();
            var stockResult = new Dictionary<string, IEnumerable<StockPrice>>(result,StringComparer.OrdinalIgnoreCase);
            
            if (!stockResult.ContainsKey(ticker)) return NotFound();

            var response = stockResult.Where(stk => stk.Key.Equals(ticker, StringComparison.OrdinalIgnoreCase))
                            .SelectMany(stp => stp.Value);

            return Ok(response);
        }
    }
}
