using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Repository;
using Plant_BiologyEducation.Entity.Model;
using Microsoft.AspNetCore.Authorization;
using Plant_BiologyEducation.Entity.DTO.User;
using PlantBiologyEducation.Entity.DTO.User;

namespace Plant_BiologyEducation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepo;
        private readonly IMapper _mapper;

        public UserController(UserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }


        // GET: api/User
        [HttpGet("getAllUsers")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllUsers()
        {
            var users = _userRepo.GetAllUsers();
            var usersDTO = _mapper.Map<List<UserDTO>>(users);
            return Ok(usersDTO);
        }

        // GET: api/User
        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUsers([FromQuery] string? fullName)
        {
            var users = string.IsNullOrWhiteSpace(fullName)
                ? _userRepo.GetAllUsers()
                : _userRepo.SearchUsersByFullName(fullName);

            var usersDTO = _mapper.Map<List<UserDTO>>(users);
            return Ok(usersDTO);
        }
        [HttpGet("inactive")]
        //[Authorize(Roles = "Admin")]
        public IActionResult GetInactiveUsers()
        {
            var inactiveUsers = _userRepo.GetInactiveUsers();
            var usersDTO = _mapper.Map<List<UserDTO>>(inactiveUsers);
            return Ok(usersDTO);
        }


        [HttpGet("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public IActionResult GetUserById(Guid id)
        {
            if (!_userRepo.UserExists(id))
                return NotFound();

            var user = _userRepo.GetUserById(id);
            var userDTO = _mapper.Map<UserDTO>(user);
            return Ok(userDTO);
        }

        // POST: api/User
        [HttpPost("admin-create")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateUser([FromBody] AdminRequestDTO userRequestDTO)
        {
            if (userRequestDTO == null)
                return BadRequest("User data is required.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_userRepo.AccountExists(userRequestDTO.Account))
                return Conflict("Account already exists.");
            // Map DTO to Entity
            var user = _mapper.Map<User>(userRequestDTO);
             
            // Tạo ID mới cho user
            user.User_Id = Guid.NewGuid();

            if (!_userRepo.CreateUser(user))
            {
                return StatusCode(500, "Something went wrong while saving user.");
            }

            // Trả về user vừa tạo (không bao gồm password)
            var userDTO = _mapper.Map<UserDTO>(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.User_Id }, userDTO);
        }



        // PUT: api/User/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Student,Teacher")]

        public IActionResult UpdateUser(Guid id, [FromBody] UserRequestDTO userRequestDTO)
        {
            if (userRequestDTO == null)
                return BadRequest("User data is required.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_userRepo.UserExists(id))
                return NotFound();

            // Lấy user hiện tại từ database
            var existingUser = _userRepo.GetUserById(id);

            // Map các thay đổi từ DTO sang Entity (giữ nguyên ID)
            _mapper.Map(userRequestDTO, existingUser);
            existingUser.User_Id = id; // Đảm bảo ID không bị thay đổi

            if (!_userRepo.UpdateUser(existingUser))
            {
                return StatusCode(500, "Something went wrong while updating user.");
            }

            return Ok(new { message = "Update User successfully" });
        }
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateUserStatus(Guid id, [FromBody] UserStatusUpdateDTO statusDto)
        {
            var user = _userRepo.GetUserById(id);
            if (user == null)
                return NotFound("User not found.");

            user.IsActive = statusDto.IsActive;

            var result = _userRepo.UpdateUser(user);
            if (!result)
                return StatusCode(500, "Failed to update user status.");

            return Ok(new
            {
                message = "User status updated successfully.",
                userId = user.User_Id,
                newStatus = user.IsActive ? "Active" : "Inactive"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(Guid id)
        {
            if (!_userRepo.UserExists(id))
                return NotFound();

            // Retrieve the User object by ID
            var userToDelete = _userRepo.GetUserById(id);
            if (userToDelete == null)
                return NotFound();

            // Pass the User object to DeleteUser
            if (!_userRepo.DeleteUser(userToDelete))
            {
                return StatusCode(500, "Something went wrong while deleting user.");
            }

            return Ok(new { message = "Delete User successfully" });
        }
    }
}