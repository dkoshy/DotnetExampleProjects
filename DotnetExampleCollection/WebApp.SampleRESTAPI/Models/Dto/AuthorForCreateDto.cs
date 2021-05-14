using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApp.SampleRESTAPI.Models.Dto;

namespace ebApp.SampleRESTAPI.Models.Dto
{
    public class AuthorForCreateDto : AuthorForManupulationDto
    {
        public AuthorForCreateDto()
        {
            Courses = new List<CourseForCreateDto>();
        }

      
        public ICollection<CourseForCreateDto> Courses { get; set; }

    }
}