using AutoMapper;
using FinalProject.Api.Models;
using FinalProject.Core.DTOs;
using FinalProject.Core.Entities;
using FinalProject.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        // GET: api/<TurnController>
        [HttpGet]
        //[Authorize(Policy = "AdminOnly")]
        public ActionResult<IEnumerable<User>> Get()
        {
            var user = _userService.GetAll();
            var userDTO = _mapper.Map<IEnumerable<UserDTO>>(user);
            return Ok(userDTO);
        }


        [HttpGet("{id}")]
        //[Authorize(Policy = "AdminOnly")]
        public ActionResult<User> Get(int id)
        {
            var user = _userService.GetById(id);
            var userDTO = _mapper.Map<UserDTO>(user);

            if (userDTO is null)
            {
                return NotFound();
            }
            return Ok(userDTO);
        }

        //[HttpGet("{id}/folders")]
        //[Authorize]
        //public ActionResult<IEnumerable<FolderDTO>> GetCoursesByUserId(int id)
        //{
        //    var courses = _userService.GetFoldersByTeacherId(id); // הנחה שיש מתודה כזו בשירות המשתמשים
        //    var courseDTOs = _mapper.Map<IEnumerable<FolderDTO>>(courses);

        //    if (courseDTOs == null || !courseDTOs.Any())
        //    {
        //        return NotFound();
        //    }
        //    return Ok(courseDTOs);
        //}
        [HttpGet("{id}/folders")]
        //[Authorize]
        public ActionResult GetCoursesByUserId(int id)
        {
            var courses = _userService.GetFoldersByTeacherId(id);
            var courseDTOs = _mapper.Map<IEnumerable<FolderDTO>>(courses);

            if (courseDTOs == null || !courseDTOs.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "לא נמצאו תיקיות למשתמש זה.",
                    data = new List<FolderDTO>()
                });
            }

            return Ok(new
            {
                success = true,
                message = "תיקיות נטענו בהצלחה.",
                data = courseDTOs
            });
        }


        [HttpGet("{id}/usersFolders")]
        //[Authorize(Policy = "StudentOnly")]
        public ActionResult<IEnumerable<UserDTO>> GetUsersCoursesByUserId(int id)
        {
            var users = _userService.GetUsersFoldersByFolderId(id); // הנחה שיש מתודה כזו בשירות המשתמשים
            var userDTOs = _mapper.Map<IEnumerable<UserDTO>>(users);

            if (userDTOs == null || !userDTOs.Any())
            {
                return NotFound();
            }
            return Ok(userDTOs);
        }
    }
}