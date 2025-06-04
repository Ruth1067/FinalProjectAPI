using AutoMapper;
using FinalProject.Core.DTOs;
using FinalProject.Core.Entities;
using FinalProject.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserDTO>> Get()
        {
            var users = _userService.GetAll();
            var userDTOs = _mapper.Map<IEnumerable<UserDTO>>(users);
            return Ok(userDTOs);
        }

        [HttpGet("{id}")]
        public ActionResult<UserDTO> Get(int id)
        {
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound();

            var userDTO = _mapper.Map<UserDTO>(user);
            return Ok(userDTO);
        }

        [HttpGet("{id}/folders")]
        public ActionResult GetCoursesByUserId(int id)
        {
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound("משתמש לא נמצא.");

            IEnumerable<Folder> courses;

            if (user.Role == "Teacher")
            {

                courses = _userService.GetFoldersByTeacherId(id);
            }
            else
            {


                courses = _userService.GetPurchasedCoursesByUserId(id);
            }

            var courseDTOs = _mapper.Map<IEnumerable<FolderDTO>>(courses);

            return Ok(new
            {
                success = true,
                message = "תיקיות נטענו בהצלחה.",
                data = courseDTOs
            });
        }

        [HttpGet("{id}/usersFolders")]
        public ActionResult<IEnumerable<UserDTO>> GetUsersCoursesByUserId(int id)
        {
            var users = _userService.GetUsersFoldersByFolderId(id);
            var userDTOs = _mapper.Map<IEnumerable<UserDTO>>(users);

            if (userDTOs == null || !userDTOs.Any())
            {
                return NotFound();
            }

            return Ok(userDTOs);
        }

        [HttpPost("{userId}/purchase/{folderId}")]
        public async Task<IActionResult> PurchaseCourse(int userId, int folderId)
        {
            var success = await _userService.PurchaseCourseAsync(userId, folderId);

            if (!success)
                return Conflict("הרכישה כבר קיימת.");

            return Ok(new
            {
                success = true,
                message = "הרכישה נשמרה בהצלחה"
            });
        }

    }
}
