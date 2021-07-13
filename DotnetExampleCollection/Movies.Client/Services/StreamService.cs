using Movies.Client.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Client.Services
{
    public class StreamService : IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });
   

        public StreamService()
        {
            _httpClient.BaseAddress = new Uri("http://localhost:57863");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();

        
        }

        public async Task Run()
        {
            //await WorkingwithStreams();
            //await WorkingWithStreamOnCreation();
            //await WorkingwithStreamandCompression();
            await WorkingWithStreamOnPost();
        }  
        
        private async Task WorkingwithStreams()
        {
            Poster poster = null;

            var request = new HttpRequestMessage(
                   HttpMethod.Get,
                   $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
           
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            //request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                poster = stream.ReadFromStream<Poster>();
            }

            if (poster != null)
            {
                Console.WriteLine($"{poster.Id} ----------- {poster.MovieId}");
            }
     
        }

        private async Task WorkingwithStreamandCompression()
        {
            Poster poster;
            var request = new HttpRequestMessage(HttpMethod.Get,
                     $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            try
            {
                response.EnsureSuccessStatusCode();
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using(var reader = new StreamReader(stream))
                    {
                        using(var jsotestreder  = new JsonTextReader(reader))
                        {
                            var jsonSerliser = new JsonSerializer();
                            poster =  jsonSerliser.Deserialize<Poster>(jsotestreder);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            if(poster != null)
            {
                Console.WriteLine($"{poster.Id} ------- {poster.MovieId}");
            }


        }

        private async Task WorkingWithStreamOnCreation()
        {
            Poster newlyAdded;
            var radombytes = new byte[578900];
            var random = new Random();
            random.NextBytes(radombytes);

            var posterForCreation = new PosterForCreation
            {
                Name = "new Movie Posetr",
                Bytes = radombytes
            };

            var memorystream = new MemoryStream();

            using (var streamWriter = new StreamWriter(memorystream , new UTF8Encoding(), 8192, true))
            {
                using(var jsonwriter = new JsonTextWriter(streamWriter))
                {
                    var jsonSerlizer = new JsonSerializer();
                    jsonSerlizer.Serialize(jsonwriter, posterForCreation);
                    jsonwriter.Flush();
                }
            }
        
            memorystream.Seek(0, SeekOrigin.Begin);
           

            var request = new HttpRequestMessage(HttpMethod.Post,
                           "api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new StreamContent(memorystream);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(request);

            try
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                newlyAdded = JsonConvert.DeserializeObject<Poster>(content);
            }
            catch
            {
                throw;
            }

            if(response != null)
            {
                Console.WriteLine("Poster is created");
            }

        }

        private async Task WorkingWithStreamOnPost()
        {
            Poster poster = null;
            var bytesGenerated = new byte[789667];
            var random = new Random();
            random.NextBytes(bytesGenerated);

            var posterForPost = new PosterForCreation()
            {
                Name = "True Lies",
                Bytes = bytesGenerated
            };

            //create a stream for content
            var memoryStreamcontent = new MemoryStream();
            memoryStreamcontent.WriteToStream(posterForPost);

            memoryStreamcontent.Seek(0, SeekOrigin.Begin);

            var request = new HttpRequestMessage(HttpMethod.Post,
             "api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StreamContent(memoryStreamcontent);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(request);

            using (var content = await response.Content.ReadAsStreamAsync())
            {
                using (var streamReader = new StreamReader(content))
                {
                    using (var jsonreader = new JsonTextReader(streamReader))
                    {
                        var jsonserlizer = new JsonSerializer();

                        poster = jsonserlizer.Deserialize<Poster>(jsonreader);
                    }
                }
            }


            if (poster != null)
            {
                Console.WriteLine($"{poster.Id} ----- {poster.MovieId}");
            }

        }

      
    }
}
