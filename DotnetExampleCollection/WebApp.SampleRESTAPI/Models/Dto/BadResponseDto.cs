using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.SampleRESTAPI.Models.Dto
{
    public class BadResponseDto
    {
        public BadResponseDto()
        {
            Errors = new List<string>();
        }

        public string ResourceUrl { get; set; }

        public string ResourceId { get; set; }

        public ICollection<string> Errors { get; set; }

    }
}
