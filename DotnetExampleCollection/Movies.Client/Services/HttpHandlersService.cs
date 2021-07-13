using Movies.Client.Models;
using Newtonsoft.Json;
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
    public class HttpHandlersService : IIntegrationService
    {
        private static HttpClient _nonConstuctorHttpclient = new HttpClient(
             new RetryPolicyHandler(new HttpClientHandler() {
                 AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate

             }, 2));

        private readonly IHttpClientFactory _httpClientFactory;

        public HttpHandlersService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            _nonConstuctorHttpclient.BaseAddress = new Uri("http://localhost:57863");
            _nonConstuctorHttpclient.Timeout = new TimeSpan(0, 0, 30);
            _nonConstuctorHttpclient.DefaultRequestHeaders.Clear();
            
        }

        public async Task Run()
        {
            // await GetMovieDetailsWithRetryPattern();
            await GetMovieDetailsWithTimeOut();
        } 

        public async Task GetMovieDetailsWithRetryPattern(CancellationToken token = default)
        {
            var httpclient = _httpClientFactory.CreateClient(Commoncontants.movieapiName);

            var request = new HttpRequestMessage(HttpMethod.Get, "api/movie");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            var response = await httpclient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                if(response.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("Movie is not Found . Please check the uri");
                    return;
                }
                else if(response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Api is not Authorized ");
                    return;
                }

                response.EnsureSuccessStatusCode();
            }

         
            var content = await response.Content.ReadAsStringAsync();

            var movies = JsonConvert.DeserializeObject<List<Movie>>(content);

            if(movies != null)
            {
                Console.WriteLine($"Number of Moview - {movies.Count}");
            }
            
        }

        public async Task GetMovieDetailsWithTimeOut(CancellationToken token = default)
        {
            var httpclient = _httpClientFactory.CreateClient(Commoncontants.movieapiName);

            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            var response = await httpclient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);

            if (!response.IsSuccessStatusCode)
            {
                if(response.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("Incorrect URI - resource not found");
                    return;
                }
                else if(response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("User is not undefined");
                    return;
                }
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync();
            var movies = JsonConvert.DeserializeObject<List<Movie>>(content);

            if(movies != null)
            {
                Console.WriteLine($"Number of Movies {movies.Count}");
            }

        }
             

    }
}
