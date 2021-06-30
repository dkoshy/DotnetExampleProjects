using StockAnalyzer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StockAnalizer.ConsoleApp
{
    public class StockStreamingService : IStockStreamingService
    {
        public async  IAsyncEnumerable<StockPrice> GetStockStreamingPrice(CancellationToken token = default)
        {
            using var stream =  new StreamReader(File.OpenRead(@"StockPrices_small.csv"));

            await stream.ReadLineAsync();

            string line;
            
            while((line = await stream.ReadLineAsync()) != null)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                var segments = line.Split(',');
                for (var i = 0; i < segments.Length; i++) segments[i] = segments[i].Trim('\'', '"');

                var price = new StockPrice
                {
                    Ticker = segments[0],
                    TradeDate = DateTime.ParseExact(segments[1], "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                    Volume = Convert.ToInt32(segments[6], CultureInfo.InvariantCulture),
                    Change = Convert.ToDecimal(segments[7], CultureInfo.InvariantCulture),
                    ChangePercent = Convert.ToDecimal(segments[8], CultureInfo.InvariantCulture),
                };

                yield return price;
            }

        }
    }

    public class MockStockStreamingService : IStockStreamingService
    {
        public async  IAsyncEnumerable<StockPrice> GetStockStreamingPrice(CancellationToken token = default)
        {
            await Task.Delay(500);
            yield return new StockPrice { Ticker = "STOC1", Change = -1, High = 5, Low = 2, ChangePercent = 1, Volume = 250 };
            await Task.Delay(500);
            yield return new StockPrice { Ticker = "STOC2", Change = -1, High = 5, Low = 2, ChangePercent = 1, Volume = 250 };
            await Task.Delay(500);
            yield return new StockPrice { Ticker = "STOC3", Change = -1, High = 5, Low = 2, ChangePercent = 1, Volume = 250 };
            await Task.Delay(500);
            yield return new StockPrice { Ticker = "STOC4", Change = -1, High = 5, Low = 2, ChangePercent = 1, Volume = 250 };
            await Task.Delay(500);
            yield return new StockPrice { Ticker = "STOC5", Change = -1, High = 5, Low = 2, ChangePercent = 1, Volume = 250 };
            await Task.Delay(500);
            yield return new StockPrice { Ticker = "STOC6", Change = -1, High = 5, Low = 2, ChangePercent = 1, Volume = 250 };

        }
    }

    public interface IStockStreamingService
    {
        IAsyncEnumerable<StockPrice> GetStockStreamingPrice(CancellationToken token = default);
      
    }


}
