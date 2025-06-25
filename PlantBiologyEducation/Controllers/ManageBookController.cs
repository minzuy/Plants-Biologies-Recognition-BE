using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Entity.DTO.Management;
using Plant_BiologyEducation.Entity.Model;
using Plant_BiologyEducation.Repository;
using System.Security.Claims;

namespace Plant_BiologyEducation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ManageBookController : ControllerBase
    {
        private readonly ManageBookRepository _manageBookRepository;
        private readonly IMapper _mapper;

        public ManageBookController(ManageBookRepository manageBookRepository, IMapper mapper)
        {
            _manageBookRepository = manageBookRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var entities = await _manageBookRepository.GetAllAsync();
                var result = _mapper.Map<List<ManageBookDTO>>(entities);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("{bookId}")]
        public async Task<IActionResult> GetByBookId(Guid bookId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var entity = await _manageBookRepository.GetByIdAsync(userId, bookId);

                if (entity == null)
                    return NotFound(new { success = false, message = "Management record not found" });

                var result = _mapper.Map<ManageBookDTO>(entity);
                return Ok(new { success = true, data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] ManageBookRequestDTO dto)
        {
            try
            {
                if (dto?.BookId == Guid.Empty)
                    return BadRequest(new { success = false, message = "Invalid BookId" });

                var userId = GetUserIdFromToken();

                // Check if record already exists
                var existingRecord = await _manageBookRepository.GetByIdAsync(userId, dto.BookId);
                if (existingRecord != null)
                    return Conflict(new { success = false, message = "Management record already exists" });

                var entity = new ManageBook
                {
                    User_Id = userId,
                    Book_Id = dto.BookId,
                    UpdatedDate = DateTime.UtcNow
                };

                await _manageBookRepository.AddAsync(entity);
                var result = _mapper.Map<ManageBookDTO>(entity);
                return CreatedAtAction(nameof(GetByBookId), new { bookId = dto.BookId },
                    new { success = true, data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }

        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier ||
                                                           c.Type == "sub" ||
                                                           c.Type == "userId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("Valid UserId not found in token.");

            return userId;
        }
    }
}