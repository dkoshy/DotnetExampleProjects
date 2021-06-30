using Newtonsoft.Json;
using StockAnalyzer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockAnalizer.ConsoleApp
{
    public interface IStockService
    {
        Task<IEnumerable<StockPrice>> GetStockTiker(string ticker, CancellationToken token);
    }

    public class StockService :IStockService
    {

        private static string gettickerUrl = "https://localhost:44392/api/stocks/";
        public StockService()
        {

        }

        public async Task<IEnumerable<StockPrice>> GetStockTiker(string ticker, CancellationToken token)
        {
            var url =  gettickerUrl + ticker;

            using(var httpclinet = new HttpClient())
            {
                var response = await  httpclinet.GetAsync(url, token);
                var stockPriceData = new List<StockPrice>();
                try
                {
                    response.EnsureSuccessStatusCode();
                    var stockPrice = await response.Content.ReadAsStringAsync();
                    stockPriceData = JsonConvert.DeserializeObject<List<StockPrice>>(stockPrice);
                }
                catch(Exception ex)
                {
                    throw;
                }
                return stockPriceData;
            }
        }

       
    }
}
