using System;

namespace WebApp.SampleRESTAPI.Models.Dto
{
    public class CourseBaseDto
    {
        public Guid Id { get; set; }
        public string title { get; set; }
        public string description { get; set; }

    }
}