using AutoMapper;
using ebApp.SampleRESTAPI.Models.Dto;
using Library.API.Entities;
using WebApp.SampleRESTAPI.Models.Dto;
using WebApp.SampleRESTAPI.Utilities;

namespace WebApp.SampleRESTAPI.AppProfiles
{
    public class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            CreateMap<Library.API.Entities.Author, AuthorDto>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.GetAge()));

            CreateMap<AuthorForCreateDto, Author>();
            CreateMap<AuthorForUpdateDto, Author>();
        }
    }
}
