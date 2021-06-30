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
        private static Random random = new Random();
        static object syncRoot = new object();

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
            //await AMockAsyncClassExample();

            // await ProcessStocksUsingParallelExtention();
            // await ProcessStockUsingParallelForEach();

            //await ProcessStocksUsingParallelFor();
            //await ProcessStocksUsingParallelFor();
            //await ProcessStocksUsingParallelFor();

            //await ProcessstockWithProgressIndication();

            // await ProessTaskFroAFuctionWhereItNotUsingAsyncPattern();
            // await NodePadProcessExited();
            // await  AttchigAndDetachingChildTask();
            await AsyncEnumerableExample();
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

        public static async Task ProcessStocksUsingParallelExtention()
        {
            var stockService = new StockService();
            var tickers = new List<string> { "GPX", "GOOGL" };
            cancelationtockenSource = new CancellationTokenSource();

            var taskList = new List<Task<IEnumerable<StockPrice>>>();

            foreach(var ticker in tickers)
            {
                var stockTask = stockService.GetStockTiker(ticker, cancelationtockenSource.Token);
                taskList.Add(stockTask);
            }

            var allstocks = (await Task.WhenAll(taskList)).SelectMany(stock => stock);

            //Parallel Extenstion for Parrallel Execution.
            Parallel.Invoke( new ParallelOptions { MaxDegreeOfParallelism = 2 } ,
                () =>
                {
                    Debug.WriteLine("Starting Operation 1");
                    CalculateExpensiveComputation(allstocks);
                    Debug.WriteLine("Completed Operation 1");

                },
                () =>
                {
                    Debug.WriteLine("Starting Operation 2");
                    CalculateExpensiveComputation(allstocks);
                    Debug.WriteLine("Completed Operation 2");
                },
                () =>
                {
                    Debug.WriteLine("Starting Operation 3");
                    CalculateExpensiveComputation(allstocks);
                    Debug.WriteLine("Completed Operation 3");
                },
                () =>
                {
                    Debug.WriteLine("Starting Operation 4");
                    CalculateExpensiveComputation(allstocks);
                    Debug.WriteLine("Completed Operation 4");
                });

            Console.WriteLine($"Number of Stocks Processed {allstocks.Count()}");

        }

        public static async Task ProcessStockUsingParallelForEach()
        {
            var stockService = new StockService();
            var tickers = new List<string> { "GPX", "GOOGL" , "ADC", "AIG", "BOCH", "MSFT" };
            cancelationtockenSource = new CancellationTokenSource();

            var taskList = new List<Task<IEnumerable<StockPrice>>>();

            foreach (var ticker in tickers)
            {
                var stockTask = stockService.GetStockTiker(ticker, cancelationtockenSource.Token);
                taskList.Add(stockTask);
            }

            var allstocks = await Task.WhenAll(taskList);

            var stockCalculatedResults = new ConcurrentBag<StockCalculation>();

            var parallelData = Parallel.ForEach(allstocks, new ParallelOptions { MaxDegreeOfParallelism = 2 }, (stocks ,state) =>
             {
                 var ticker = stocks.First().Ticker;

                 Debug.WriteLine($"Start Processing {ticker} ");

               /*  if(ticker == "BOCH")
                 {
                     Debug.WriteLine($"Break for {ticker}");
                     state.Break();
                     //state.Stop();
                     return;
                 } */

                 var result = CalculateExpensiveComputation(stocks);

                 var data = new StockCalculation
                 {
                     Result = result,
                     Ticker = ticker
                 };

                 stockCalculatedResults.Add(data);

                 Debug.WriteLine($"End Processing {ticker}");
             });

            Console.WriteLine($" Ran To Completion : {parallelData.IsCompleted} , Lowest Break : {parallelData.LowestBreakIteration}");
            foreach(var result in stockCalculatedResults)
            {
                Console.WriteLine($"{result.Result} -- {result.Ticker}");
            }
        }

        public static async Task ProcessStocksUsingParallelFor()
        {
            var stockService = new StockService();
            var tickers = new List<string> { "GPX", "GOOGL", "ADC", "AIG", "BOCH", "MSFT" };
            cancelationtockenSource = new CancellationTokenSource();
            var  allTaskList = new List<Task<IEnumerable<StockPrice>>>();

            foreach(var ticker in tickers)
            {
                var stockTask = stockService.GetStockTiker(ticker, cancelationtockenSource.Token);
                allTaskList.Add(stockTask);
            }

            //var allLoadedTask = (await Task.WhenAll(allTaskList)).SelectMany(stock => stock).ToArray();
            var allstocks = await Task.WhenAll(allTaskList);
            decimal  total = 0;
            // Parallel.For(0, allLoadedTask.Length, i =>
            //{
            //    //Interlocked.Add(ref total, (int)Compute(allLoadedTask[i]));

            //    lock(syncRoot)
            //    {
            //        total += Compute(allLoadedTask[i]);
            //    }
            //});
            Parallel.ForEach(allstocks, new ParallelOptions { MaxDegreeOfParallelism=3 } , (stocks,state) =>
            {
                decimal value = 0;

                foreach(var stock in stocks)
                {
                    value += Compute(stock);
                }

                lock(syncRoot)
                {
                    total += value;
                }
            });


            Console.WriteLine($"Total : {total}");
        }

        public static async Task ProcessstockWithProgressIndication()
        {
             var tickers = new List<string> { "GPX", "GOOGL", "ADC", "AIG", "BOCH", "MSFT" };
            cancelationtockenSource = new CancellationTokenSource();
            var progress = new Progress<IEnumerable<StockPrice>>();

            progress.ProgressChanged += (object sender, IEnumerable<StockPrice> e) =>
            {
                  Console.WriteLine($"Process completed for {e.First().Ticker}");
            };

            var allTaskList =  LoadTickers(tickers, progress);

            var allstockData = await Task.WhenAll(allTaskList);

            foreach(var stocks in allstockData)
            {
                var stock = stocks.First();
                Console.WriteLine($"Ticker {stock.Ticker} ----- {stock.Volume}");
            }
        }

        public static async Task ProessTaskFroAFuctionWhereItNotUsingAsyncPattern()
        {
            var stockPriceDetails = await  GetstockFor("GOOGL");

            foreach(var stock in stockPriceDetails)
            {
                Console.WriteLine($"Volume - { stock.Volume}");

            }
        }

        public static async Task NodePadProcessExited()
        {
            await NotePadProcess();
            Console.WriteLine("Note Pad Process exited , Tahnk you");
        }

        public static async Task  AttchigAndDetachingChildTask()
        {
           Debug.WriteLine("Starting");
           await  Task.Factory.StartNew(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    Debug.WriteLine("Completing 1");

                },TaskCreationOptions.AttachedToParent);

                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    Debug.WriteLine("Completing 2");

                }, TaskCreationOptions.AttachedToParent);

                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(10000);
                    Debug.WriteLine("Completing 3");
                },TaskCreationOptions.AttachedToParent);

               
            });
            Debug.WriteLine("Completing Main Task");
        }

        public static async Task AsyncEnumerableExample()
        {
            var mockservice = new MockStockStreamingService();

            await foreach(var ticker in mockservice.GetStockStreamingPrice())
            {
                Console.WriteLine($"{ticker.Ticker} ---- {ticker.Volume}");
            }

        }



        private static Task NotePadProcess()
        {
            var source = new TaskCompletionSource<object>();
            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo("Notepad.exe")
                {
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };

            process.Exited += ( sender,  e) =>
            {
                source.SetResult(null);
            };
            process.Start();

            return source.Task;

        }

        private static Task<IEnumerable<StockPrice>> GetstockFor(string ticker)
        {
            var source = new TaskCompletionSource<IEnumerable<StockPrice>>();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
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

                    source.SetResult(data.Where(s => s.Ticker == ticker));
                }
                catch(Exception ex)
                {
                    source.SetException(ex);
                }
            });

           return source.Task;
        }

        private static List<Task<IEnumerable<StockPrice>>> LoadTickers( List<string> tickers, IProgress<IEnumerable<StockPrice>> progress =  null)
        {
            var stockService = new StockService();
            var allTask = new List<Task<IEnumerable<StockPrice>>>();
            foreach (var ticker in tickers)
            {
                var stockTask = stockService.GetStockTiker(ticker, cancelationtockenSource.Token);

                stockTask.ContinueWith(t =>
                {
                    progress?.Report(t.Result);
                    return t.Result;
                });

                allTask.Add(stockTask);
            }

            return allTask;
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

        private static decimal CalculateExpensiveComputation(IEnumerable<StockPrice> stocks)
        {
            Thread.Yield();

            var computedValue = 0m;

            foreach (var stock in stocks)
            {
                for (int i = 0; i < stocks.Count() - 2; i++)
                {
                    for (int a = 0; a < random.Next(50, 60); a++)
                    {
                        computedValue += stocks.ElementAt(i).Change + stocks.ElementAt(i + 1).Change;
                    }
                }
            }

            return computedValue;
        }

        private static decimal Compute(StockPrice stock)
        {
            Thread.Yield();

            decimal x = 0;
            for (var a = 0; a < 10; a++)
            {
                for (var b = 0; b < 20; b++)
                {
                    x += a + stock.Change;
                }
            }

            return x;
        }
    }
}
