using Movies.Client.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Movies.Client.Services
{
    public class CRUDService : IIntegrationService
    {
        public static HttpClient _httpClient = new HttpClient();

        public CRUDService()
        {
            _httpClient.BaseAddress = new Uri("http://localhost:57863");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();

            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json",0.9));
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
           
        }

        public async Task Run()
        {
            //await GetMoviedetails();
            // await GetMoviewUsingReqMessage();

            // await CrreateMovieDetails();
            //await UpdateMoviewDetails();
            // await DeleteMovieDetails();
        }

        private static async Task  GetMoviedetails()
        {
           var response = await  _httpClient.GetAsync("/api/movies");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var movieDetails = new List<Movie>();

            if(response.Content.Headers.ContentType.MediaType == "application/json")
            {
                 movieDetails = JsonSerializer.Deserialize<List<Movie>>(content, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase

                });

            }
            else if(response.Content.Headers.ContentType.MediaType == "application/xml")
            {
                var serlizer = new XmlSerializer(typeof(List<Movie>));
                movieDetails = (List<Movie>)serlizer.Deserialize(new StringReader(content));

            }
           

            movieDetails?.ForEach(m =>
            {
                Console.WriteLine($"{m.Title} {m.Director} {m.ReleaseDate}");
            });
        }

        private static async Task GetMoviewUsingReqMessage()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/movies");
          
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         

            var response = await _httpClient.SendAsync(request);
            var movieDetails = new List<Movie>();
            try
            {
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                movieDetails = JsonSerializer.Deserialize<List<Movie>>(content, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }
            catch(Exception ex)
            {
                throw;
            }

            movieDetails?.ForEach(m =>
            {
                Console.WriteLine($"{m.Title}  {m.Genre}");
            });

        }

        private static async Task CrreateMovieDetails()
        {
            var newMovie = new MovieForCreation
            {
                Title = "Reservoir Dogs",
                Description = "After a simple jewelry heist goes terribly wrong, the " +
                  "surviving criminals begin to suspect that one of them is a police informant.",
                ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
                DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                Genre = "Crime, Drama",
                
            };

            var jsonMovie = JsonSerializer.Serialize(newMovie); 

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(jsonMovie);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(request);
            Movie createdMovie = null;

            try
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                createdMovie = JsonSerializer.Deserialize<Movie>(content, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }); 

            }
            catch(Exception ex)
            {
                throw;
            }

            if(createdMovie.Id != default)
            {
                Console.WriteLine("Movie Created");
            }
         }

        private static async Task UpdateMoviewDetails()
        {
            var movieforUpdate = new MovieForUpdate
            {
                Title = "Pulp Fiction",
                Description = "The movie with Zed.",
                DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
                Genre = "Crime, Drama"
            };
            var jsonMovie = JsonSerializer.Serialize(movieforUpdate);

            var request = new HttpRequestMessage(HttpMethod.Put, "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new StringContent(jsonMovie);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(request);

            Movie Updatedmovie = null;

            try
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                Updatedmovie = JsonSerializer.Deserialize<Movie>(content, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            }
            catch (Exception ex)
            {
                throw;
            }

            if (Updatedmovie.Id != default)
            {
                Console.WriteLine("Movie updated");
            }

        }

        private static async Task DeleteMovieDetails()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");
            request.Headers.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                throw;
            }

            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Data is Deleted ");

        }
    }
}