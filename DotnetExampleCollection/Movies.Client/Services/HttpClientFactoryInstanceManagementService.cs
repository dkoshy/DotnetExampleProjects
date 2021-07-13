using Movies.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Movies.Client.Services
{
    public class HttpClientFactoryInstanceManagementService : IIntegrationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly MovieClient _movieClient;
        private static CancellationTokenSource _CancellationTokenSource;

        public HttpClientFactoryInstanceManagementService(IHttpClientFactory httpClientFactory, 
            MovieClient movieClient)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException();
            _movieClient = movieClient;
            _CancellationTokenSource = new CancellationTokenSource();
        }

        public async Task Run()
        {
            // await GetMovieUsingHttpFactroy(_CancellationTokenSource.Token);
            //await GetMovieDetailsUsingNamedInstance(_CancellationTokenSource.Token);
            await GetMoviesFromTypedInstance();
        }

        private async Task GetMoviesFromTypedInstance()
        {
            var movies = (await _movieClient.GetAllMovie()).ToList();

            if(movies != null)
            {
                Console.WriteLine($"number of Movies {movies.Count}");
            }
        }

        private async Task GetMovieDetailsUsingNamedInstance(CancellationToken token)
        {
            List<Movie> movies = null;
            var httpclinet = _httpClientFactory.CreateClient(Commoncontants.movieapiName);

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var response = await httpclinet.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
                response.EnsureSuccessStatusCode();
                
                using(var stream = await response.Content.ReadAsStreamAsync())
                {
                    movies = stream.ReadFromStream<List<Movie>>();
                }
            }
            catch
            {
                throw;
            }
         
            if(movies != null)
            {
                Console.WriteLine($"Movie count - {movies.Count}");
            }
            
         
        }

        private async Task GetMovieUsingHttpFactroy(CancellationToken token)
        {
            List<Movie> movies = null;

            var httpclient =   _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get,
                "/api/Movies");
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            try
            {
                var response = await httpclient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                    token);
                response.EnsureSuccessStatusCode();

                using(var stream = await response.Content.ReadAsStreamAsync())
                {
                    movies = stream.ReadFromStream<List<Movie>>();
                }
            }
            catch
            {
                throw;
            }

            if(movies != null)
            {
                Console.WriteLine($"Number of Movies -  {movies.Count}");
            }

        }
    }
}
