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
   public  class TestableAPIExample
    {
        private readonly HttpClient _httpClient;

        public TestableAPIExample(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task GetMovieTestAPI(CancellationToken token = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);

            if (!response.IsSuccessStatusCode)
            {
                if(response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ResourseNotFoundException();
                }
                else if(response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorisedRequestException();
                }

                response.EnsureSuccessStatusCode();
            }
            

            var content = await response.Content.ReadAsStringAsync();

            var movies = JsonConvert.DeserializeObject<List<Movie>>(content);

            if(movies != null)
            {
                Console.WriteLine($"Number of Movies {movies.Count}") ;
            }
        } 
    }
}
