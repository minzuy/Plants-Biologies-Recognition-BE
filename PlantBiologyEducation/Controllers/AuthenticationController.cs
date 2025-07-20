using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Login([FromBody] LoginDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Identifier) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Vui lòng nhập đầy đủ thông tin.");
            }

            // Tìm user theo Email hoặc Username
            var allUsers = _userRepo.GetAllUsers(); // Dùng UserRepository thay vì _context
            var user = allUsers.FirstOrDefault(u =>
                (u.Email == request.Identifier || u.Account == request.Identifier)
                && u.Password == request.Password); // So sánh mật khẩu trực tiếp (không hash)

            if (user == null)
            {
                return Unauthorized("Tên đăng nhập/email hoặc mật khẩu không đúng.");
            }

            var jwt = _jwtService.GenerateToken(user);

            var response = new
            {
                token = jwt,
                user.User_Id,
                user.FullName,
                user.Email,
                user.Account,
                user.Role,
                Message = "Đăng nhập thành công"
            };

            return Ok(response);
        }


        [HttpPost("verify-google-token")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyGoogleTokenOnly([FromBody] GoogleLoginDTO request)
        {
            if (string.IsNullOrEmpty(request.IdToken))
                return BadRequest("Missing idToken");

            var payload = await VerifyGoogleToken(request.IdToken);

            if (payload.Aud != "974321576275-3s0ev7qs9q8fju3j1954lmlnrq3g3t3f.apps.googleusercontent.com")
                return Unauthorized("Token was not issued for this application");


            if (payload == null)
                return Unauthorized("Invalid Google token");

            return Ok(new
            {
                email = payload.Email,
                name = payload.Name,
                message = "Google token is valid"
            });
        }


        [HttpPost("google-signin")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleAuth([FromBody] GoogleLoginDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.IdToken))
                return BadRequest("Missing Google idToken");


            GoogleJsonWebSignature.Payload payload;
            try
            {
                // Xác thực idToken từ Google Console
                payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
            }
            catch
            {
                return Unauthorized("Invalid Google token");
            }

            // Kiểm tra người dùng đã tồn tại trong hệ thống chưa
            var user = _userRepo.GetUserByEmail(payload.Email);

            // Nếu chưa có thì tạo mới người dùng
            if (user == null)
            {
                user = new User
                {
                    User_Id = Guid.NewGuid(),
                    Account = payload.Email,
                    Password = "", // Google users không cần mật khẩu
                    FullName = payload.Name,
                    Email = payload.Email,
                    Role = "Student",
                    IsActive = true
                };

                if (!_userRepo.CreateUser(user))
                    return StatusCode(500, "Could not create user");
            }

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                token = token,
                user = new
                {
                    id = user.User_Id,
                    name = user.FullName,
                    email = user.Email,
                    role = user.Role
                },
                message = "Đăng nhập bằng Google thành công"
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
