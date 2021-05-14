using System.ComponentModel.DataAnnotations;

namespace WebApp.SampleRESTAPI.Models.Dto
{
    public class CourseUpdateDto : CourseForManupulationDto
    {
        [Required]
        public override string description { get; set ; }
      
    }
}