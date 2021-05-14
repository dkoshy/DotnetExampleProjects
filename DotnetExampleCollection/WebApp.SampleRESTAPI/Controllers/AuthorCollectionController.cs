using AutoMapper;
using CourseLibrary.API.Services;
using ebApp.SampleRESTAPI.Models.Dto;
using Library.API.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApp.SampleRESTAPI.Models.Dto;
using WebApp.SampleRESTAPI.Utilities;

namespace WebApp.SampleRESTAPI.Controllers
{
    [Route("api/authorcollections")]
    [ApiController]
    public class AuthorCollectionController : ControllerBase
    {
        private readonly ICourseLibraryRepository _service;
        private readonly IMapper _mapper;
        private const string GetAuthorCollectionRouteName = "GetAuthorCollection";

        public AuthorCollectionController(ICourseLibraryRepository serviec , IMapper mapper)
        {
            _service = serviec;
            _mapper = mapper;
        }

        [HttpGet("({Ids})" ,Name = GetAuthorCollectionRouteName)]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthorCollection(    
            [FromRoute]
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> Ids)
        {
            if(Ids == null)
            {
                return BadRequest();

            }

            var authorDetails = _service.GetAuthors(Ids);

            if(authorDetails == null || authorDetails.ToList().Count == 0)
            {
                return NotFound();
            }

            var authors = _mapper.Map<IEnumerable<AuthorDto>>(authorDetails);

            return Ok(authors);
        }

        [HttpPost]
        public ActionResult<IEnumerable<AuthorDto>> CreateAuthorCollection(IEnumerable<AuthorForCreateDto> authorForCreateDtos)
        {
            var authorCollection = _mapper.Map<IEnumerable<Author>>(authorForCreateDtos);

           foreach(var author in authorCollection)
            {
                _service.AddAuthor(author);
            }

            _service.Save();

            var newAuthors = _mapper.Map<IEnumerable<AuthorDto>>(authorCollection);

            return CreatedAtRoute(GetAuthorCollectionRouteName, 
                new { Ids = string.Join("," ,newAuthors.Select(a => a.Id) ) }, newAuthors);
        }
    }
}
