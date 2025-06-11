using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Entity.DTO;
using Plant_BiologyEducation.Entity.Model;
using Plant_BiologyEducation.Repository;

namespace Plant_BiologyEducation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TakingTestController : ControllerBase
    {
        private readonly TakingTestRepository _takingTestRepo;
        private readonly IMapper _mapper;
        private readonly UserRepository _userRepo;


        public TakingTestController(TakingTestRepository repo, IMapper mapper, UserRepository userRepo  )
        {
            _takingTestRepo = repo;
            _mapper = mapper;
            _userRepo = userRepo;
        }

        // POST: Học sinh làm kiểm tra
        [HttpPost]
        public async Task<IActionResult> TakeTest([FromBody] SubmitTestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Bỏ xác thực token - sử dụng userId từ DTO
                if (dto.UserId == Guid.Empty)
                    return BadRequest("UserId is required.");

                if (!_userRepo.UserExists(dto.UserId))
                    return NotFound();

                var entity = _mapper.Map<TakingTest>(dto);
                entity.TakingDate = DateTime.UtcNow;

                var result = await _takingTestRepo.AddTakingTest(entity);
                return result ? Ok("Submitted successfully.") : StatusCode(500, "Failed to save taking test.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("my-results")]
        public async Task<IActionResult> GetUserResults([FromQuery] Guid userId)
        {
            try
            {
                var results = await _takingTestRepo.GetTakingsByUser(userId);
                if (!_userRepo.UserExists(userId))
                    return NotFound();
                var dtos = _mapper.Map<IEnumerable<TakingTestDTO>>(results);
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("test/{testId}")]
        public async Task<IActionResult> GetResultsByTest(string testId)
        {
            try
            {
                if (string.IsNullOrEmpty(testId))
                    return BadRequest("TestId is required.");

                var dtos = await _takingTestRepo.GetTakingsByTest(testId);
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}