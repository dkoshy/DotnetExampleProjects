using System;
using System.ComponentModel.DataAnnotations;
using WebApp.SampleRESTAPI.Models.Dto;

namespace WebApp.SampleRESTAPI.Validation
{
    public class CourseForValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var course = (CourseForManupulationDto)value;

            if(course.title.Equals(course.description, StringComparison.InvariantCultureIgnoreCase))
            {
                return new ValidationResult(ErrorMessage, new string[] { "CourseForCreateDto" });
            }

            return ValidationResult.Success;
        }
    }
}
