using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Entity.DTO;
using Plant_BiologyEducation.Repository;
using Plant_BiologyEducation.Entity.Model;
using Microsoft.AspNetCore.Authorization;

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
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userRepo.GetAllUsers(); // Lấy danh sách User từ DB
            var usersDTO = _mapper.Map<List<UserDTO>>(users); // Mapping sang DTO
            return Ok(usersDTO);
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

        // POST: api/User
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateUser([FromBody] UserRequestDTO userRequestDTO)
        {
            if (userRequestDTO == null)
                return BadRequest("User data is required.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Map DTO to Entity
            var user = _mapper.Map<User>(userRequestDTO);

            // Tạo ID mới cho user
            user.Id = Guid.NewGuid();

            if (!_userRepo.CreateUser(user))
            {
                return StatusCode(500, "Something went wrong while saving user.");
            }

            // Trả về user vừa tạo (không bao gồm password)
            var userDTO = _mapper.Map<UserDTO>(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, userDTO);
        }

        // PUT: api/User/{id}
        [HttpPut("{id}")]
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
            existingUser.Id = id; // Đảm bảo ID không bị thay đổi

            if (!_userRepo.UpdateUser(existingUser))
            {
                return StatusCode(500, "Something went wrong while updating user.");
            }

            return NoContent();
        }

        // Fix for CS1503: Argument 1: cannot convert from 'System.Guid' to 'Plant_BiologyEducation.Entity.Model.User'
        // The DeleteUser method in UserRepository expects a User object, but the code is passing a Guid.
        // To fix this, retrieve the User object by its ID before calling DeleteUser.

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

            return NoContent();
        }
    }
}