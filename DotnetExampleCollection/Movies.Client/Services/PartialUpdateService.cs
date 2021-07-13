using Microsoft.AspNetCore.JsonPatch;
using Movies.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Client.Services
{
    public class PartialUpdateService : IIntegrationService
    {
        public static HttpClient _httpClient = new HttpClient();

        public PartialUpdateService()
        {
            _httpClient.BaseAddress = new Uri("http://localhost:57863");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task Run()
        {
            //await PartialMovieUpdates();
            await PatchMovieshortcut();
        }         

        public static async Task PartialMovieUpdates()
        {
            var MovieForPatch = new JsonPatchDocument<MovieForUpdate>();
            MovieForPatch.Replace(m => m.Title, "Updated Title");
            MovieForPatch.Remove(m => m.Description);

            var jsonPatchDoc = JsonConvert.SerializeObject(MovieForPatch);

            var requestMessage = new HttpRequestMessage(HttpMethod.Patch, 
                "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");
            requestMessage.Headers.Clear();
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            requestMessage.Content = new StringContent(jsonPatchDoc);
            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json-patch+json");

            var response =  await _httpClient.SendAsync(requestMessage);

            var content = await response.Content.ReadAsStringAsync();

            var movie = JsonConvert.DeserializeObject<Movie>(content);

            if(movie != null)
            {
                Console.WriteLine("Patch is Done");
            }

        }

        public static async Task PatchMovieshortcut()
        {
            Movie updatedMovie = null;

            var MovieForPatch = new JsonPatchDocument<MovieForUpdate>();
            MovieForPatch.Replace(m => m.Title, "Title is Modified");
            MovieForPatch.Remove(m => m.Description);

            var jsonPatchDoc = JsonConvert.SerializeObject(MovieForPatch);

            var response = await _httpClient.PatchAsync("api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b",
                new StringContent(jsonPatchDoc, Encoding.UTF8, "application/json-patch+json")
                );
            try
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                updatedMovie = JsonConvert.DeserializeObject<Movie>(content);
            }
            catch (Exception)
            {
                throw;
            }

            if(updatedMovie != null)
            {
                Console.WriteLine("Movie Got Updated");
            }
            

        }


    }
}
