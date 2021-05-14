using AutoMapper;
using Library.API.Entities;
using WebApp.SampleRESTAPI.Models.Dto;

namespace WebApp.SampleRESTAPI.AppProfiles
{
    public class CourseProfile :Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseBaseDto>();
            CreateMap<CourseForCreateDto, Course>();
            CreateMap<CourseUpdateDto, Course>().ReverseMap();
              

        }
    }
}
