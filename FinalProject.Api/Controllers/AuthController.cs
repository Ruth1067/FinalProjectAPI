////using FinalProject.Core.DTOs;
////using FinalProject.Core.Entities;
////using FinalProject.Core.Repositories;


//////[Route("api/[controller]")]
//////[ApiController]
//////public class UsersController : ControllerBase
//////{
//////    private readonly IUserRepository _repository;

//////    public UsersController(IUserRepository repository)
//////    {
//////        _repository = repository;
//////    }

//////    [HttpGet("{id}")]
//////    public ActionResult<UserDTO> GetById(int id)
//////    {
//////        var user = _repository.GetById(id);
//////        if (user == null) return NotFound();
//////        return new UserDTO
//////        {
//////            Id = user.Id,
//////            Username = user.Username,
//////            Email = user.Email,
//////            PhoneNumber = user.PhoneNumber
//////        };
//////    }

//////    [HttpPost]
//////    public ActionResult<UserDTO> Create(User user)
//////    {
//////        _repository.Add(user);
//////        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
//////    }
//////}
////using Microsoft.AspNetCore.Mvc;
////using Microsoft.AspNetCore.Mvc;

////[Route("api/[controller]")]
////[ApiController]
////public class UsersController : ControllerBase
////{
////    private readonly IUserRepository _repository;

////    public UsersController(IUserRepository repository)
////    {
////        _repository = repository;
////    }

////    [HttpGet]
////    public IEnumerable<UserDTO> Get()
////    {
////        return 
////    }

////    [HttpGet("{id}")]
////    public ActionResult<UserDTO> GetById(int id)
////    {
////        var user = _repository.GetById(id);
////        if (user == null) return NotFound();
////        return new UserDTO
////        {
////            Id = user.Id,
////            Username = user.Username,
////            Email = user.Email,
////            PhoneNumber = user.PhoneNumber
////        };
////    }

////    [HttpPost]
////    public ActionResult<UserDTO> Create(User user)
////    {
////        _repository.Add(user);
////        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
////    }
////}
//using AutoMapper;
//using FinalProject.Api.Models;
//using FinalProject.Core.DTOs;
//using FinalProject.Core.Entities;
//using FinalProject.Core.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace FinalProject.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UserController : ControllerBase
//    {

//        private readonly IUserService _userService;
//        private readonly IMapper _mapper;

//        public UserController(IUserService userService, IMapper mapper)
//        {
//            _userService = userService;
//            _mapper = mapper;
//        }

//        // GET: api/<TurnController>
//        [HttpGet]
//        public ActionResult<IEnumerable<User>> Get()
//        {
//            var user = _userService.GetAll();
//            var userDTO = _mapper.Map<IEnumerable<UserDTO>>(user);
//            return Ok(userDTO);
//        }

//        // GET api/<TurnController>/5
//        [HttpGet("{id}")]
//        public ActionResult<User> Get(int id)
//        {
//            var user = _userService.GetById(id);
//            var userDTO = _mapper.Map<UserDTO>(user);

//            if (userDTO is null)
//            {
//                return NotFound();
//            }
//            return Ok(userDTO);
//        }

//        //POST api/<TurnController>
//        [HttpPost]
//        public ActionResult Post([FromBody] UserPostModel value)
//        {
//            var user = new User { Id = 111, Username = "value.IdNurse", Password = "value.DateOfTurn", Email = "aaa", PhoneNumber = "aaa" };

//            var newUser = _userService.PostUser(user);

//            if (value == null)
//            {
//                return NotFound();
//            }

//            return Ok(newUser);
//        }

//        // PUT api/<TurnController>/5
//        [HttpPut("{d}")]
//        public ActionResult Put(string d, [FromBody] UserPostModel value)
//        {
//            var user = new User { Id = 111, Username = "value.IdNurse", Password = "value.DateOfTurn", Email = "aaa", PhoneNumber = "aaa" };

//            var newUser = _userService.PutUser(d, user);

//            if (newUser is null)
//                return NotFound();

//            return Ok(newUser);
//        }

//        // DELETE api/<TurnController>/5
//        [HttpDelete("{id}")]
//        public ActionResult Delete(int id)
//        {
//            var user = _userService.DeleteUser(id);

//            if (user is null)
//                return NotFound();
//            else
//                return Ok(user);
//        }
//    }
//}
using FinalProject;
using FinalProject.Api.Models;
using FinalProject.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DataContext _dataContext;

    public AuthController(IConfiguration configuration, DataContext dataContext)
    {
        _configuration = configuration;
        _dataContext = dataContext;
    }


    [HttpPost("/api/login")]
    public IActionResult Login([FromBody] LoginModel loginModel)
    {
        var user = _dataContext.Users?.FirstOrDefault(u => u.Email == loginModel.Email && u.Password == loginModel.Password);
        if (user is not null)
        {
            var jwt = CreateJWT(user);
            //AddSession(user);
            return Ok(jwt);
        }
        return Unauthorized();
    }

    //[HttpPost("/api/teacherLogin")]
    //public IActionResult TeacherLogin([FromBody] UserModel loginModel)
    //{
    //    var user = _dataContext.Users?.FirstOrDefault(u => u.Email == loginModel.Email && u.Password == loginModel.Password&&u.IsTeacher==loginModel.IsTeacher&&loginModel.IsTeacher==true);
    //    if (user is not null)
    //    {
    //        var jwt = CreateJWT(user);
    //        //AddSession(user);
    //        return Ok(jwt);
    //    }
    //    return Unauthorized();
    //}

    [HttpPost("/api/register")]
    public IActionResult Register([FromBody] RegisterModel registerModel)
    {
        var newUser = new User {Username = registerModel.Username, Password = registerModel.Password, Email = registerModel.Email, PhoneNumber = registerModel.PhoneNumber };
        _dataContext.Users?.Add(newUser);
        _dataContext.SaveChanges();
        var jwt = CreateJWT(newUser);
        return Ok(jwt);
    }

    private object CreateJWT(User user)
    {
        var claims = new List<Claim>()
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("name", user.Username),
                    new Claim("email", user.Email),
                    new Claim("phoneNumber", user.PhoneNumber)
                };

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:Key")));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokeOptions = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>("JWT:Issuer"),
            audience: _configuration.GetValue<string>("JWT:Audience"),
            claims: claims,
            expires: DateTime.Now.AddDays(30),
            signingCredentials: signinCredentials
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return new { Token = tokenString };
    }
}
