using Movies.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Movies.Client
{
    public class MovieClient 
    {
        private readonly HttpClient _httpClient;

        public static string Name => "MovieAPIClient";

        public MovieClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("http://localhost:57863/");
            httpClient.Timeout = new TimeSpan(0, 0, 30);
            httpClient.DefaultRequestHeaders.Clear();

            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Movie>> GetAllMovie( CancellationToken token = default)
        {
            IEnumerable<Movie> movies = null;

            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));


            try
            {
                var response = await _httpClient.SendAsync(request, 
                    HttpCompletionOption.ResponseHeadersRead, token);
                response.EnsureSuccessStatusCode();

                using (var stream = await  response.Content.ReadAsStreamAsync())
                {
                    movies = stream.ReadFromStream<IEnumerable<Movie>>();
                }

            }
            catch
            {
                throw;
            }

            return movies;
        }

        public Task<HttpResponseMessage> GetMovieById(CancellationToken token = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                "api/movies/D8663E5E-7494-4F81-8739-6E0DE1BEA711");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            var response = _httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,
                token);

            return response;
        }

        public Task<HttpResponseMessage> CreateMovie(CancellationToken token = default)
        {
            var movieforCreation = new MovieForCreation
            {
                Description ="Something New",
                DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
              
            };

            var jsonMovie = JsonConvert.SerializeObject(movieforCreation);

            var request = new HttpRequestMessage(HttpMethod.Post, "api/movies");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Commoncontants.ApiJsonContent));
            request.Content = new StringContent(jsonMovie);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(Commoncontants.ApiJsonContent);

            return _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
        }

    }
}
