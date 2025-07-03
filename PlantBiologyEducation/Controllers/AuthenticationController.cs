using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Entity.DTO.User;
using Plant_BiologyEducation.Entity.Model;
using Plant_BiologyEducation.Repository;
using Plant_BiologyEducation.Service;
using PlantBiologyEducation.Entity.DTO.User;


namespace Plant_BiologyEducation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly UserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        public AuthenticationController(UserRepository userRepo, IMapper mapper, JwtService jwtService)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _jwtService = jwtService;
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginDTO loginDto)
        {
            var user = _userRepo.GetAllUsers()
                                .FirstOrDefault(u => u.Account == loginDto.Account && u.Password == loginDto.Password);

            if (user == null)
                return Unauthorized("Invalid credentials.");

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                Token = token,
                User = new
                {
                    userId = user.User_Id,
                }
            });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] UserRequestDTO userRequestDTO)
        {
            if (userRequestDTO == null)
                return BadRequest("User data is required.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Chỉ cho phép đăng ký với Role là Student hoặc Teacher
            var allowedRoles = new[] { "Student"};
            if (!allowedRoles.Contains(userRequestDTO.Role))
                return BadRequest("You are only allowed to register with the role of 'Student'.");

            // Kiểm tra trùng Account
            if (_userRepo.GetAllUsers().Any(u => u.Account == userRequestDTO.Account))
                return Conflict("Account already exists.");

            var user = _mapper.Map<User>(userRequestDTO);
            user.User_Id = Guid.NewGuid();

            if (!_userRepo.CreateUser(user))
                return StatusCode(500, "Something went wrong while registering user.");

            var userDTO = _mapper.Map<UserDTO>(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.User_Id }, userDTO);


        }
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequestDTO requestPassword)
        {
            if (string.IsNullOrWhiteSpace(requestPassword.Account) || string.IsNullOrWhiteSpace(requestPassword.NewPassword))
                return BadRequest("Account and new password are required.");

            var user = _userRepo.GetUserByAccount(requestPassword.Account);

            if (user == null)
                return NotFound("Account not found.");

            user.Password = requestPassword.NewPassword; // TODO: Hash nếu cần

            if (!_userRepo.UpdateUser(user))
                return StatusCode(500, "Failed to update password.");

            return Ok("Password updated successfully.");
        }
        // GET: api/User/{id}
        [HttpGet("{id}")]
        public IActionResult GetUserById(Guid id)
        {
            if (!_userRepo.UserExists(id))
                return NotFound();

            var user = _userRepo.GetUserById(id);
            var userDTO = _mapper.Map<UserDTO>(user);
            return Ok(userDTO);
        }
    }
}
