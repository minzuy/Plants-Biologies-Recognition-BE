using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Entity.DTO.User;
using Plant_BiologyEducation.Entity.Model;
using Plant_BiologyEducation.Repository;
using Plant_BiologyEducation.Service;
using PlantBiologyEducation.Entity.DTO.Authen;
using System.Text.Json;


namespace Plant_BiologyEducation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class AuthenticationController : Controller
    {
        private readonly UserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly EmailService _emailService;
        public AuthenticationController(UserRepository userRepo, IMapper mapper, JwtService jwtService, IPasswordHasher<User> passwordHasher, EmailService emailService)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginDTO loginDto)
        {
            var user = _userRepo.GetAllUsers()
                .FirstOrDefault(u =>
                    u.Account.Equals(loginDto.Identifier, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Equals(loginDto.Identifier, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                return Unauthorized("Invalid credentials.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);
            if (result != PasswordVerificationResult.Success)
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


        [HttpPost("google-test")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [AllowAnonymous]
        public IActionResult GoogleLoginTest()
        {
            var payload = new GooglePayload
            {
                Email = "duy@gmail.com",
                Name = "Test User",
            };

            var user = _userRepo.GetUserByAccount(payload.Email);

            if (user == null)
            {
                user = new User
                {
                    User_Id = Guid.NewGuid(),
                    Account = payload.Email,
                    Password = "1234577",
                    FullName = payload.Name,
                    Role = "Student",
                    IsActive = true,
                };
                _userRepo.CreateUser(user);
            }

            var jwt = _jwtService.GenerateToken(user);

            return Ok(new
            {
                token = jwt,
                user = new
                {
                    id = user.User_Id,
                    name = user.FullName,
                    email = user.Account,
                    role = user.Role
                }
            });
        }


        [HttpPost("google")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleAuth([FromBody] GoogleLoginDTO request)
        {
            var payload = await VerifyGoogleToken(request.IdToken);

            if (payload == null || string.IsNullOrEmpty(payload.Email))
                return Unauthorized("Invalid Google token");

            var user = _userRepo.GetUserByAccount(payload.Email);

            // Nếu chưa có, tự động đăng ký
            if (user == null)
            {
                user = new User
                {
                    User_Id = Guid.NewGuid(),
                    Account = payload.Email,
                    Password = "", 
                    FullName = payload.Name,
                    Role = "Student", // mặc định
                    IsActive = true
                };

                _userRepo.CreateUser(user);
            }

            var jwt = _jwtService.GenerateToken(user);

            return Ok(new
            {
                token = jwt,
                user = new
                {
                    id = user.User_Id,
                    name = user.FullName,
                    email = user.Account,
                    role = user.Role
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

        [HttpPost("forgot-password/request")]
        [AllowAnonymous]
        public IActionResult RequestResetPassword([FromBody] ForgotPasswordRequestDTO request)
        {

            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email is required.");

            var user = _userRepo.GetUserByEmail(request.Email);
            if (user == null)
                return NotFound("Email not found.");

            string verificationCode = new Random().Next(100000, 999999).ToString();
            user.resetToken = verificationCode;

            if (!_userRepo.UpdateUser(user))
                return StatusCode(500, "Failed to update user with reset code.");

            bool emailSent = _emailService.SendVerificationCode(user.Email, verificationCode);
            if (!emailSent)
                return StatusCode(500, "Failed to send verification email.");

            return Ok("Verification code sent to your email.");
        }

        [HttpPost("forgot-password/confirm")]
        [AllowAnonymous]
        public IActionResult ConfirmResetPassword([FromBody] ForgotPasswordConfirmDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.VerificationCode) ||
                string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return BadRequest("Missing required fields.");
            }

            var user = _userRepo.GetUserByEmail(dto.Email);
            if (user == null)
                return NotFound("Email not found.");

            if (user.resetToken != dto.VerificationCode)
                return BadRequest("Invalid verification code.");

            user.Password = _passwordHasher.HashPassword(user, dto.NewPassword);
            user.resetToken = null;

            if (!_userRepo.UpdateUser(user))
                return StatusCode(500, "Failed to reset password.");

            return Ok("Password has been reset successfully.");
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

        private async Task<GooglePayload?> VerifyGoogleToken(string idToken)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}");

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<GooglePayload>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

    }

}
