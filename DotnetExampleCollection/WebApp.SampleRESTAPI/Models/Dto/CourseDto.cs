using System;
using System.Collections.Generic;

namespace WebApp.SampleRESTAPI.Models.Dto
{ 
    public class CourseDto 
    {
        public CourseDto()
        {
            courses = new List<CourseBaseDto>();
        }
       public Guid AuthorId { get; set; }

       public IList<CourseBaseDto> courses { get; set; }
    }
}