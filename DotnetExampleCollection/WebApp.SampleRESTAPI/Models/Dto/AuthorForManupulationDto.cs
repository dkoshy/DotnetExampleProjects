using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.SampleRESTAPI.Models.Dto
{
    public class AuthorForManupulationDto
    {

        [Required]
        public string FirstName { get; set; }

        public virtual string LastName { get; set; }

        [Required]
        public DateTimeOffset DateOfBirth { get; set; }

        [Required]
        public string MainCategory { get; set; }
    }
}
