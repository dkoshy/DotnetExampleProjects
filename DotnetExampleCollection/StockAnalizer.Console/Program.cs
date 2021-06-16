using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockAnalyzer.Core.Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StockAnalizer.ConsoleApp
{
    class Program
    {
       // private readonly static string requesturl = "https://localhost:44392/api/stocks/";
        private static CancellationTokenSource cancelationtockenSource = null;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Stock analizer console Application");
            //await  SearchStock("a");
            //await SearchStockwithTaskVersion();
            //SearchStockwithTaskwithcontinueOption();
            //SerachStockwithCancelationTocken();
            //SerachStockforMultipleTikerParallley();
            // await SerachStockforMultipleTikerParallley();
            // await ProcessTheStockAsWeReceveFromconcreentExection();
            await AMockAsyncClassExample();
        }

       

        public static async Task SearchStock(string ticker)
        {
            var service = new StockService();
            var stockprices = new List<StockPrice>();

            cancelationtockenSource = new CancellationTokenSource();


            //if (cancelationtockenSource != null)
            //{
            //    cancelationtockenSource.CancelAfter(10);
                
            //}

            cancelationtockenSource.Token.Register(() =>
            {
                Console.WriteLine("Api call is cancelled");
                cancelationtockenSource = null;
            });

            try
            {
                var data = await service.GetStockTiker("GOOGL", cancelationtockenSource.Token);
                stockprices = data.ToList();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Some exception is happend");
            }
            

            foreach (var stock in stockprices)
            {
                Console.WriteLine($"{stock.Ticker} , {stock.Change} , {stock.Volume} ");
            }

            

        }

        public static async Task SearchStockwithTaskVersion()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var count = 0;

            var data = await Task.Run(() =>
             {
                 var lines = File.ReadAllLines(@"StockPrices_Small.csv");
                 var data = new List<StockPrice>();

                 foreach (var line in lines.Skip(1))
                 {
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
                     data.Add(price);
                 }


                 return data;
             });

            foreach (var stockPrice in data)
            {
                if (count > 10) { break; }

                Console.WriteLine($"{stockPrice.Change} , {stockPrice.Ticker} , {stockPrice.Volume}");

                count++;
            }

            stopWatch.Stop();
            Console.WriteLine($"Time Taken {stopWatch.ElapsedMilliseconds}");

        }

        public static void SearchStockwithTaskwithcontinueOption()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var count = 0;
            var exceptionsCount = 0;
            var exceptionMessage = "";

            var linesCsv = Task.Run(() =>
           {
               var lines = File.ReadAllLines(@"StockPrices_Small1.csv");
               return lines;

           });

            var exception = linesCsv.ContinueWith(t =>
             {
                 exceptionsCount = t.Exception.InnerExceptions.Count;
                 exceptionMessage = string.Join(",", t.Exception.InnerExceptions.Select(e => e.Message));

             }, TaskContinuationOptions.OnlyOnFaulted);


            var processLines = linesCsv.ContinueWith(t =>
           {
               var lines = t.Result;
               var data = new List<StockPrice>();

               foreach (var line in lines.Skip(1))
               {
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
                   data.Add(price);
               }

               return data;

           }, TaskContinuationOptions.OnlyOnRanToCompletion);

            try
            {

                foreach (var stockPrice in processLines.Result)
                {
                    if (count > 10) { break; }

                    Console.WriteLine($"{stockPrice.Change} , {stockPrice.Ticker} , {stockPrice.Volume}");

                    count++;
                }
            }
            catch (Exception ex)
            {

                if (exceptionsCount > 0)
                {
                    Console.WriteLine($"Process faild with below exceptons ");
                    Console.WriteLine($"Count : {exceptionsCount}");
                    Console.WriteLine($"Message : {exceptionMessage}");
                    Console.WriteLine($"Time Taken {stopWatch.ElapsedMilliseconds}");
                    return;
                }
            }

            stopWatch.Stop();
            Console.WriteLine($"Time Taken {stopWatch.ElapsedMilliseconds}");

        }

        public static void SerachStockwithCancelationTocken()
        {
            var exceptionList = new List<string>();
            var ISRequestCanceled = false;

            cancelationtockenSource = new CancellationTokenSource();

            cancelationtockenSource.Token.Register(() =>
            {
                ISRequestCanceled = true;
            });


            var lineTask = GetStockLines(cancelationtockenSource.Token);

            lineTask.ContinueWith(t =>
            {
                exceptionList = t.Exception.InnerExceptions.Select(e => e.Message).ToList();

            }, TaskContinuationOptions.OnlyOnFaulted);

            var pullStocksDataTask = lineTask.ContinueWith(t =>
            {
                var lines = t.Result;
                var data = new List<StockPrice>();

                foreach (var line in lines.Skip(1))
                {
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
                    data.Add(price);
                }

                return data;
            },TaskContinuationOptions.OnlyOnRanToCompletion);

            //Initate Cancellation
            Thread.Sleep(30);

            if(cancelationtockenSource != null)
            {
                cancelationtockenSource.Cancel();
                cancelationtockenSource = null;
            }
          
            //Initate Cancellation 

            try
            {
                var stockDetails = pullStocksDataTask.Result;
                Console.WriteLine($"Data fetched Succfully , Number of Data Fetched : {stockDetails.Count}");
                Console.WriteLine($"{(ISRequestCanceled ? "Cancelation is Requested" : "")}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception Count : {exceptionList.Count}");
            }
            
        }

        public static async Task SerachStockforMultipleTikerParallley()
        {
            var service = new StockService();
            var tikers = new List<string> { "GPX", "GOOGL"};
            var allTasks = new List<Task<IEnumerable<StockPrice>>>();

            cancelationtockenSource = new CancellationTokenSource();

            foreach(var tiker in tikers)
            {
                var task =  service.GetStockTiker(tiker, cancelationtockenSource.Token);
                allTasks.Add(task);
            }

            var taskDelay = Task.Delay(10000);
            var allStocks =  Task.WhenAll(allTasks);

            var anyTask = await Task.WhenAny(taskDelay, allStocks);

            if(anyTask == taskDelay)
            {
                cancelationtockenSource.Cancel();
                cancelationtockenSource = null;
                throw new TimeoutException("Task execution is exeedes the expected Time");
            }

            var data = allStocks.Result.SelectMany(stk => stk);

           foreach(var item in data)
            {
                Console.WriteLine($"--{item.Ticker} ---{item.Volume}--");
            }
        }

        public static async Task ProcessTheStockAsWeReceveFromconcreentExection()
        {
            var tickers = new List<string> { "GPX", "GOOGL" };
            var stockService = new StockService();
            var taskList = new List<Task<IEnumerable<StockPrice>>>();
            var taskResult = new ConcurrentBag<StockPrice>();
          
            cancelationtockenSource = new CancellationTokenSource();

            foreach(var ticker in tickers)
            {
                var stockTask = stockService.GetStockTiker(ticker, cancelationtockenSource.Token)
                                  .ContinueWith(t =>
                                  {
                                     var fisrtFive =  t.Result.Take(5);
                                      foreach(var r in fisrtFive)
                                      {
                                          taskResult.Add(r);

                                          Console.WriteLine($"--{r.Ticker} ---{r.Volume}--");
                                      }

                                      return t.Result;
                                   });

                taskList.Add(stockTask);
            }

            var alltaskCompleted = Task.WhenAll(taskList);

            await alltaskCompleted;

        }
        public static async Task AMockAsyncClassExample()
        {
            cancelationtockenSource = new CancellationTokenSource();
            var mockservice = new MockStockservice();

            var results = await mockservice.GetStockTiker("ongc", cancelationtockenSource.Token);

            foreach (var ticker in results)
            {
                Console.WriteLine($"{ticker.Ticker} -------- {ticker.Volume}");
            }
        }

        private static Task<List<string>> GetStockLines(CancellationToken token)
        {
            var LoadlinesTask = Task.Run(async () =>
            {
                var lines = new List<string>();
                using (var reader = new StreamReader(@"StockPrices_Small.csv"))
                {
                    var line = "";
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return lines;
                        }
                        lines.Add(line);
                    }

                }

                return lines;
            });

            return LoadlinesTask;
        }
    }
}
