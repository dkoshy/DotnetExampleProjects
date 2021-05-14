using System.ComponentModel.DataAnnotations;
using WebApp.SampleRESTAPI.Validation;

namespace WebApp.SampleRESTAPI.Models.Dto
{
    [CourseForValidation(ErrorMessage = "Title and Description should not be Same")]

    public abstract class CourseForManupulationDto
    {
        [Required]
        [MaxLength(100)]
        public string title { get; set; }

        [MaxLength(1500)]
        public virtual string description { get; set; }
    }
}
