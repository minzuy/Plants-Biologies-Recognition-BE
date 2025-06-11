using Plant_BiologyEducation.Entity.DTO;
using Plant_BiologyEducation.Entity.Model;
using Plant_BiologyEducation.Repository;
using Plant_BiologyEducation.Service;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Plant_BiologyEducation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TestController : ControllerBase
    {
        private readonly TestRepository _testRepository;
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;

        public TestController(TestRepository testRepository, UserRepository userRepository, IMapper mapper)
        {
            _testRepository = testRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        //// GET: api/Test
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var tests = _testRepository.GetAllTests();
                var result = _mapper.Map<IEnumerable<TestDTO>>(tests);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search/creator/{creatorName}")]
        public IActionResult SearchByCreatorName(string creatorName)
        {
            try
            {
                if (string.IsNullOrEmpty(creatorName))
                    return BadRequest("Creator name is required for search.");

                var tests = _testRepository.GetTestsByCreatorName(creatorName);
                var result = _mapper.Map<IEnumerable<TestDTO>>(tests);

                return Ok(new
                {
                    SearchTerm = creatorName,
                    Count = result.Count(),
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // GET: api/Test/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Test ID is required.");

                if (!_testRepository.TestExists(id))
                    return NotFound($"Test with ID {id} not found.");

                var test = _testRepository.GetTestById(id);
                var result = _mapper.Map<TestDTO>(test);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Test
        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Create([FromBody] TestRequestDTO testRequestDTO)
        {
            try
            {
                if (testRequestDTO == null)
                    return BadRequest("Test data is required.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Kiểm tra nếu CreatorId không tồn tại trong bảng Users
                if (!_userRepository.UserExists(testRequestDTO.CreatorId))
                {
                    return BadRequest("Invalid CreatorId. User does not exist.");
                }

                // Map DTO to Entity
                var test = _mapper.Map<Test>(testRequestDTO);

                // Tạo ID tự động với 6 chữ số
                string newId;
                int attempts = 0;
                do
                {
                    newId = TestService.GenerateRandomTestId(); // Sử dụng TestService
                    attempts++;
                    if (attempts > 100) // Prevent infinite loop
                    {
                        return StatusCode(500, "Unable to generate unique test ID after multiple attempts.");
                    }
                } while (_testRepository.TestExists(newId));

                test.Id = newId;

                // Set DateCreated nếu không được cung cấp hoặc là giá trị mặc định
                if (testRequestDTO.DateCreated == default(DateTime) || testRequestDTO.DateCreated == DateTime.MinValue)
                {
                    test.DateCreated = DateTime.UtcNow;
                }

                if (!_testRepository.CreateTest(test))
                {
                    return StatusCode(500, "Something went wrong while saving test.");
                }

                // Reload test sau khi tạo để lấy đầy đủ thông tin
                var createdTest = _testRepository.GetTestById(test.Id);
                if (createdTest == null)
                {
                    return StatusCode(500, "Failed to retrieve created test.");
                }

                var result = _mapper.Map<TestDTO>(createdTest);
                return CreatedAtAction(nameof(GetById), new { id = test.Id }, result);
            }
            catch (Exception ex)
            {
                // Log chi tiết lỗi để debug
                return StatusCode(500, $"Internal server error: {ex.Message}. Inner exception: {ex.InnerException?.Message}");
            }
        }

        // PUT: api/Test/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Update(string id, [FromBody] TestRequestDTO testRequestDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Test ID is required.");

                if (testRequestDTO == null)
                    return BadRequest("Test data is required.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_testRepository.TestExists(id))
                    return NotFound($"Test with ID {id} not found.");

                // Kiểm tra CreatorId có tồn tại không
                if (!_userRepository.UserExists(testRequestDTO.CreatorId))
                {
                    return BadRequest("Invalid CreatorId. User does not exist.");
                }

                // Lấy test hiện tại từ database
                var existingTest = _testRepository.GetTestById(id);
                if (existingTest == null)
                    return NotFound($"Test with ID {id} not found.");

                // Map các thay đổi từ DTO sang Entity (Id sẽ không bị thay đổi)
                _mapper.Map(testRequestDTO, existingTest);
                existingTest.Id = id; // Đảm bảo ID không bị thay đổi

                if (!_testRepository.UpdateTest(existingTest))
                {
                    return StatusCode(500, "Something went wrong while updating test.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}. Inner exception: {ex.InnerException?.Message}");
            }
        }

        // DELETE: api/Test/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Test ID is required.");

                if (!_testRepository.TestExists(id))
                    return NotFound($"Test with ID {id} not found.");

                var test = _testRepository.GetTestById(id);
                if (test == null)
                    return NotFound($"Test with ID {id} not found.");

                if (!_testRepository.DeleteTest(test))
                {
                    return StatusCode(500, "Something went wrong while deleting test.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}. Inner exception: {ex.InnerException?.Message}");
            }
        }
    }
}