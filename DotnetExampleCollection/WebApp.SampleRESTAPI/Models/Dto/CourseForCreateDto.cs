using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApp.SampleRESTAPI.Validation;

namespace WebApp.SampleRESTAPI.Models.Dto
{
    

    public class CourseForCreateDto  : CourseForManupulationDto //IValidatableObject
    {
       

      /*  public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.Equals(title, description, StringComparison.InvariantCultureIgnoreCase))
            {
                yield return new ValidationResult("Title and Description Should not Match"
                    , new string[] { "CourseForCreateDto" }
                    );
             }
               
        } */
    }
}
