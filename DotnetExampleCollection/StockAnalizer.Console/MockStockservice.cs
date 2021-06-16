using StockAnalyzer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockAnalizer.ConsoleApp
{
    public class MockStockservice : IStockService
    {
        public async Task<IEnumerable<StockPrice>> GetStockTiker(string ticker, CancellationToken token)
        {
            var stocks = new List<StockPrice>
           {
               new StockPrice{ Ticker="MRF" , Change=-2 , ChangePercent=1  , Volume=1000 },
               new StockPrice{ Ticker="MRF" , Change=-10 , ChangePercent=3  , Volume=1400 },
               new StockPrice{ Ticker="MRF" , Change= 10 , ChangePercent=11  , Volume=1300 },
               new StockPrice{ Ticker="MRF" , Change=6 , ChangePercent=12  , Volume=1020 },
               new StockPrice{ Ticker="MRF" , Change=20 , ChangePercent=15  , Volume=100 },
               new StockPrice{ Ticker="ONGC" , Change=1 , ChangePercent=5  , Volume=102 },
               new StockPrice{ Ticker="ONGC" , Change=-2 , ChangePercent=8  , Volume=300 }
           };

           return  await Task.FromResult(stocks.Where(t => t.Ticker.Equals(ticker,StringComparison.OrdinalIgnoreCase)));
        }
    }
}
