using Movies.Client.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Movies.Client.Services
{
    public class CancellationService : IIntegrationService
    {
        public static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public static HttpClient _httpclient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });
     

        public CancellationService()
        {
            _httpclient.BaseAddress = new Uri("http://localhost:57863");
            //_httpclient.Timeout = new TimeSpan(0, 0, 30);
            _httpclient.Timeout = new TimeSpan(0, 0, 1);
            _httpclient.DefaultRequestHeaders.Clear();

           
        }
        public async Task Run()
        {
            _cancellationTokenSource.CancelAfter(1000);
            //await WorkingWithCancelationSenario(_cancellationTokenSource.Token);
            await WorkingwithTimeOutSScenario();

            _cancellationTokenSource.Token.Register(() =>
            {
                Console.WriteLine("Cancel API Call ");
            });

        }

        private static async Task WorkingWithCancelationSenario(CancellationToken token)
        {
            Trailer trailer = null;
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            var resposne = await _httpclient.SendAsync(request,
                   HttpCompletionOption.ResponseHeadersRead, token);

            try
            {
                               
                resposne.EnsureSuccessStatusCode();

                using(var stream = await resposne.Content.ReadAsStreamAsync())
                {
                    trailer = stream.ReadFromStream<Trailer>();
                }
            }
            catch
            {
                throw;
            }

            if( trailer != null)
            {
                Console.WriteLine($"{trailer.Name} ------ {trailer.MovieId}");
            }
        }

        private static async Task WorkingwithTimeOutSScenario()
        {
            Trailer trailer = null;

            var request = new HttpRequestMessage(HttpMethod.Get, 
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            try
            {
                var response = await _httpclient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    trailer = stream.ReadFromStream<Trailer>();
                }
            }
            catch(OperationCanceledException ex)
            {
                Console.WriteLine($"Operation is Cancelled {ex}");
            }

            if(trailer != null)
            {
                Console.WriteLine($"{trailer.Id}-----{trailer.MovieId}");
            }

        }

    }
}
