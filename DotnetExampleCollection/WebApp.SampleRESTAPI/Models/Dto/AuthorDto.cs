using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace WebApp.SampleRESTAPI.Models.Dto
{
    public class AuthorDto
    {
       
        public Guid Id { get; set; }
              
        public string Name { get; set; }
        
        [JsonIgnore]
        public DateTimeOffset DateOfBirth { get; set; }

        public int Age { get; set; }
       
        public string MainCategory { get; set; }
    }
}
