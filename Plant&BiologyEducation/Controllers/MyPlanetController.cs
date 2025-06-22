using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Entity.Model.MyPlant;
using Plant_BiologyEducation.Services;

namespace Plant_BiologyEducation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MyPlanetController : ControllerBase
    {
        private readonly MyPlanetService _myPlanetService;
        private readonly ILogger<MyPlanetController> _logger;

        public MyPlanetController(MyPlanetService myPlanetService, ILogger<MyPlanetController> logger)
        {
            _myPlanetService = myPlanetService;
            _logger = logger;
        }

        /// <summary>
        /// Nhận dạng loài thực vật từ hình ảnh
        /// </summary>
        /// <param name="request">Dữ liệu request chứa hình ảnh và thông tin bổ sung</param>
        /// <returns>Kết quả nhận dạng thực vật</returns>
        /// <response code="200">Thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost("identify")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(PlantIdentificationResult), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult<PlantIdentificationResult>> IdentifyPlant([FromForm] PlantIdentificationRequest request)
        {
            try
            {
                // Validate file
                if (request.Image == null || request.Image.Length == 0)
                {
                    return BadRequest(new { message = "Vui lòng chọn file ảnh để upload." });
                }

                // Validate file type
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(request.Image.ContentType.ToLower()))
                {
                    return BadRequest(new { message = "Chỉ chấp nhận file ảnh (JPG, PNG, GIF)." });
                }

                // Validate file size (5MB limit)
                if (request.Image.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { message = "Kích thước file không được vượt quá 5MB." });
                }

                // Pass organ parameter to service
                var result = await _myPlanetService.IdentifyPlantAsync(
                    request.Image,
                    request.Project ?? "all",
                    request.Organ ?? "leaf"
                );

                if (result.IsSuccess)
                {
                    _logger.LogInformation($"Successfully identified plant: {result.PlantName}");
                    return Ok(result);
                }
                else
                {
                    return Ok(new PlantIdentificationResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Không thể nhận dạng được loài thực vật từ ảnh này."
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during plant identification");
                return StatusCode(500, new { message = "Đã xảy ra lỗi trong quá trình nhận dạng.", error = ex.Message });
            }
        }

        /// <summary>
        /// Kiểm tra trạng thái health của API
        /// </summary>
        /// <returns>Trạng thái của API</returns>
        [HttpGet("health")]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Lấy danh sách các project có sẵn
        /// </summary>
        /// <returns>Danh sách project</returns>
        [HttpGet("projects")]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult GetAvailableProjects()
        {
            var projects = new[]
            {
                new { value = "all", name = "All regions", description = "Tất cả khu vực" },
                new { value = "weurope", name = "Western Europe", description = "Tây Âu" },
                new { value = "canada", name = "Canada", description = "Canada" },
                new { value = "usa", name = "USA", description = "Hoa Kỳ" },
                new { value = "cropped", name = "Cropped images", description = "Hình ảnh đã cắt" }
            };

            return Ok(new { projects, count = projects.Length });
        }

        /// <summary>
        /// Lấy danh sách các loại organ có thể nhận dạng
        /// </summary>
        /// <returns>Danh sách organ types</returns>
        [HttpGet("organs")]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult GetAvailableOrgans()
        {
            var organs = new[]
            {
                new { value = "leaf", name = "Leaf", description = "Lá" },
                new { value = "flower", name = "Flower", description = "Hoa" },
                new { value = "fruit", name = "Fruit", description = "Quả" },
                new { value = "bark", name = "Bark", description = "Vỏ cây" }
            };

            return Ok(new { organs, count = organs.Length });
        }

        /// <summary>
        /// Test API connection với PlantNet
        /// </summary>
        /// <returns>Kết quả test connection</returns>
        [HttpGet("test-connection")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                using var httpClient = new HttpClient();
                var testUrl = $"https://my-api.plantnet.org/v1/projects";
                var response = await httpClient.GetAsync(testUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Ok(new
                    {
                        status = "connected",
                        message = "Kết nối PlantNet API thành công",
                        timestamp = DateTime.UtcNow,
                        projects = content
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = "failed",
                        message = "Không thể kết nối đến PlantNet API",
                        statusCode = response.StatusCode,
                        timestamp = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Lỗi khi test connection",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}