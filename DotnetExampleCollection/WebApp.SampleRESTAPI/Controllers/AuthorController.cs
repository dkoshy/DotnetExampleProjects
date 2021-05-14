
using System;
using System.Collections.Generic;
using AutoMapper;
using CourseLibrary.API.Services;
using ebApp.SampleRESTAPI.Models.Dto;
using Library.API.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApp.SampleRESTAPI.Models.Dto;
using WebApp.SampleRESTAPI.RepositoryServices;

namespace WebApp.SampleRESTAPI.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly ICourseLibraryRepository _servce;
        private readonly IMapper _mapper;

        private const string GetAuthorByIdRouteName = "GeAuthorById";

        public AuthorController(ICourseLibraryRepository servce, IMapper mapper)
        {
            _servce = servce;
            _mapper = mapper;
        }


        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors([FromQuery] AuthorSearchFilter authorSearchFilter)
        {
            var authorResult = _servce.GetAuthors(authorSearchFilter);

            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorResult));

        }

        [HttpGet("{authorId}", Name = GetAuthorByIdRouteName)]
        [HttpHead("{authorId}")]
        public ActionResult<AuthorDto> GeAuthorById(Guid authorId)
        {
            var authorResult = _servce.GetAuthor(authorId);

            if (authorResult == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AuthorDto>(authorResult));
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor(AuthorForCreateDto authorDto)
        {
            var newAuthor = _mapper.Map<Author>(authorDto);

            _servce.AddAuthor(newAuthor);
            _servce.Save();

            var createdAuthor = _mapper.Map<AuthorDto>(newAuthor);

            return CreatedAtRoute(GetAuthorByIdRouteName,
                new { authorId = createdAuthor.Id },
                createdAuthor);
        }

        [HttpOptions]
        public IActionResult AuthorOptions()
        {
            Response.Headers.Add("Allow", "GET,POST,OPTIONS");
            return Ok();
        }

        [HttpDelete("{authorId}")]
        public IActionResult DeleteAuthor(Guid authorId)
        {
            var authorFromRepo = _servce.GetAuthor(authorId);

            if (authorFromRepo == null)
            {
                return NotFound();
            }

            _servce.DeleteAuthor(authorFromRepo);
            _servce.Save();

            return NoContent();

        }

        [HttpPut("{authorId}")]
        public ActionResult<AuthorDto> UpadteauthorDetails(Guid authorId, AuthorForUpdateDto authorForUpdateDto)
        {
            var authorFromRepo = _servce.GetAuthor(authorId);

            if(authorFromRepo == null)
            {
                return NotFound();  //upsert is also possible
            }

            _mapper.Map(authorForUpdateDto, authorFromRepo);

            _servce.UpdateAuthor(authorFromRepo);
            _servce.Save();

            var authorForResult = _mapper.Map<AuthorDto>(authorFromRepo);

            return Ok(authorForResult);
        }

        public IActionResult PatchAuthorDetails(Guid authorId , JsonPatchDocument<AuthorForUpdateDto> patchDocument)
        {
            var authorFromRepo = _servce.GetAuthor(authorId);
            if(authorFromRepo == null)
            {
                return NotFound();
            }

            var authorForPatch = _mapper.Map<AuthorForUpdateDto>(authorFromRepo);
            patchDocument.ApplyTo(authorForPatch,ModelState);

            if (!TryValidateModel(authorForPatch))
            {
                return BadRequest();
            }

            //map patcged values to original author from repo

            _mapper.Map(authorForPatch, authorFromRepo);

            _servce.UpdateAuthor(authorFromRepo);
            _servce.Save();

            return NoContent();

        }

    }
}
