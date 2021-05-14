using AutoMapper;
using CourseLibrary.API.Services;
using Library.API.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApp.SampleRESTAPI.Models.Dto;

namespace WebApp.SampleRESTAPI.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseLibraryRepository _sevice;
        private readonly IMapper _mapper;

        private const string GetAllCoursesByAuthorRouteName = "GetAllCoursesByAuthor";
        private const string GetCourseRouteName = "GetCourse";

        public CourseController(ICourseLibraryRepository sevice, IMapper mapper)
        {
            _sevice = sevice;
            _mapper = mapper;
        }

        [HttpGet(Name = GetAllCoursesByAuthorRouteName)]
        [HttpHead]
        public ActionResult<CourseDto> GetAllCourses(Guid authorId)
        {
            if (!_sevice.AuthorExists(authorId))
            {
                return NotFound();
            }
            var courseResult = new CourseDto() { AuthorId = authorId };

            var courses = _sevice.GetCourses(authorId);

            var CourseList = _mapper.Map<IEnumerable<CourseBaseDto>>(courses).ToList();

            courseResult.courses = CourseList;

            return Ok(courseResult);
        }

        [HttpGet("{courseId}", Name = GetCourseRouteName)]
        [HttpHead("{courseId}")]
        public ActionResult<CourseDto> GetCourse(Guid authorId, Guid courseId)
        {
            if (!_sevice.AuthorExists(authorId))
            {
                return NotFound();
            }

            var course = _sevice.GetCourse(authorId, courseId);

            if (course == null)
            {
                return NotFound();
            }
            var courseResult = new CourseDto { AuthorId = authorId };
            var courseData = _mapper.Map<CourseBaseDto>(course);

            courseResult.courses.Add(courseData);

            return Ok(courseResult);
        }

        [HttpPost()]
        public ActionResult<CourseDto> CreateCourseCollecton(Guid authorId, IEnumerable<CourseForCreateDto> courseForCreateDto)
        {
            if (!_sevice.AuthorExists(authorId))
            {

                var message = $"This author {authorId} is not exist.";

                var badResponse = new BadResponseDto
                {
                    ResourceId = authorId.ToString(),
                    ResourceUrl = Request.Path
                };

                badResponse.Errors.Add(message);

                return BadRequest(badResponse);
            }

            var newCourseForRepo = _mapper.Map<IEnumerable<Course>>(courseForCreateDto);


            _sevice.AddCourses(authorId, newCourseForRepo);
            _sevice.Save();

            var newCorses = _mapper.Map<IEnumerable<CourseBaseDto>>(newCourseForRepo).ToList();

            var cousreResult = new CourseDto() { AuthorId = authorId };
            cousreResult.courses = newCorses;

            return CreatedAtRoute(GetAllCoursesByAuthorRouteName, new { authorId }, cousreResult);
        }

        [HttpPut("{courseId}")]
        public ActionResult<CourseDto> UpdateCousreDetails(Guid authorId, Guid courseId,
            CourseUpdateDto courseUpdateDto)
        {
            if (!_sevice.AuthorExists(authorId))
            {
                return BadRequest();
            }

            var courseResult = new CourseDto { AuthorId = authorId };

            var courseRepo = _sevice.GetCourse(authorId, courseId);

            if (courseRepo == null)
            {
                var newCourse = _mapper.Map<Course>(courseUpdateDto);
                newCourse.Id = courseId;
                var courses = new List<Course>() { newCourse };

                _sevice.AddCourses(authorId, courses);
                _sevice.Save();


                courseResult.courses = new List<CourseBaseDto> { _mapper.Map<CourseBaseDto>(newCourse) };

                return CreatedAtRoute(GetCourseRouteName, new { authorId, courseId },
                 courseResult);
            }

            // map the entity to a CourseForUpdateDto
            // apply the updated field values to that dto
            // map the CourseForUpdateDto back to an entity
            _mapper.Map(courseUpdateDto, courseRepo);

            _sevice.UpdateCourse(courseRepo);

            _sevice.Save();

            //return updatede course details


            var updatedCourse = _mapper.Map<CourseBaseDto>(courseRepo);

            courseResult.courses.Add(updatedCourse);

            return Ok(courseResult);

        }

        [HttpPatch("{courseId}")]
        public IActionResult PatchCourseDetails(Guid authorId , Guid courseId,
            JsonPatchDocument<CourseUpdateDto> patchDocument )
        {
            if (!_sevice.AuthorExists(authorId))
            {
                return BadRequest();
            }
            var courseInRepo = _sevice.GetCourse(authorId, courseId);

            if(courseInRepo == null)
            {
                var newCourseUpdateDto = new CourseUpdateDto();
                patchDocument.ApplyTo(newCourseUpdateDto,ModelState);

                if (!TryValidateModel(newCourseUpdateDto))
                {
                    return ValidationProblem(ModelState);
                }


                var newcourse = _mapper.Map<Course>(newCourseUpdateDto);
                newcourse.Id = courseId;

                _sevice.AddCourses(authorId, new List<Course> {newcourse });
                _sevice.Save();

                var courseForResult = _mapper.Map<CourseBaseDto>(newcourse);
              
                return CreatedAtRoute(GetCourseRouteName, new { authorId, courseId }, 
                    new CourseDto { AuthorId = authorId, courses = new List<CourseBaseDto> { courseForResult} });
            }

            //create courseupdatedto from coure
            var coursePatched = _mapper.Map<CourseUpdateDto>(courseInRepo);
            patchDocument.ApplyTo(coursePatched,ModelState);

            //validation
            if (!TryValidateModel(coursePatched))
            {
                return ValidationProblem(ModelState);
            }

            //Copy patched Course to course in repo
            _mapper.Map(coursePatched, courseInRepo);
        
            _sevice.UpdateCourse(courseInRepo);
            _sevice.Save();

            return NoContent();
        }

        [HttpDelete("{courseId}")]
        public IActionResult DeleteCourse(Guid authorId, Guid courseId)
        {
            if (!_sevice.AuthorExists(authorId))
            {
                return BadRequest();
            }

            var courseFromRempo = _sevice.GetCourse(authorId, courseId);
            if(courseFromRempo == null)
            {
                return NotFound();
            }

            _sevice.DeleteCourse(courseFromRempo);
            _sevice.Save();

            return NoContent();
            
        }

        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();

            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);

        }

    }
}
