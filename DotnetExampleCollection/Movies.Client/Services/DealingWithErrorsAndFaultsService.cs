using Movies.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Client.Services
{
    public class DealingWithErrorsAndFaultsService : IIntegrationService
    {
        private readonly MovieClient _movieClient;

        public DealingWithErrorsAndFaultsService(MovieClient movieClient)
        {
            _movieClient = movieClient;
        }
        public async Task Run()
        {
            //await GetMovieandcheckforException();
            await CreateMoviewithException();
        }

        private async Task GetMovieandcheckforException()
        {
            var respose = await _movieClient.GetMovieById();
            Movie movie = null;

            if(respose == null)
            {
                return;
            }

            if (!respose.IsSuccessStatusCode)
            {
                if(respose.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("Requested Movie is not Found");
                    return;
                }
                else if(respose.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("User is not Auhorized");
                    return;
                }

                respose.EnsureSuccessStatusCode();
            }

            using(var stream = await respose.Content.ReadAsStreamAsync())
            {
                movie = stream.ReadFromStream<Movie>();
            }

            if(movie != null)
            {
                Console.WriteLine($"{movie.Id} ---------- {movie.Title}");
            }
        }

        private async Task CreateMoviewithException()
        {
            Movie movie = null;

            var response = await _movieClient.CreateMovie();

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if(response.StatusCode == HttpStatusCode.UnprocessableEntity)
                {
                    var validationError =  JsonConvert.SerializeObject(content);
                    Console.WriteLine($"{validationError}");
                    return;
                } 
                if(response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("User is not Authorised");
                    return;
                }

                response.EnsureSuccessStatusCode();
            }

            movie = JsonConvert.DeserializeObject<Movie>(content);

            if(movie != null)
            {
                Console.WriteLine($"{movie.Id} ---- {movie.Title}");
            }
        }


    }
}
