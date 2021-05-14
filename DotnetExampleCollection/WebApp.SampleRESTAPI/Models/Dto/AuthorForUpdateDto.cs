using System.ComponentModel.DataAnnotations;

namespace WebApp.SampleRESTAPI.Models.Dto
{
    public class AuthorForUpdateDto : AuthorForManupulationDto
    {
        [Required]
        public override string LastName { get; set ; }
    }
}
