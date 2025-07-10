using Microsoft.AspNetCore.Mvc;
using PlantBiologyEducation.Service; // Đảm bảo bạn đã thêm namespace này

namespace PlantBiologyEducation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PredictController : ControllerBase
    {
        private readonly PredictService _predictService;

        public PredictController(PredictService predictService)
        {
            _predictService = predictService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImgDTO request)
        {
            if (request.Image == null || request.Image.Length == 0)
            {
                return BadRequest("Vui lòng chọn một hình ảnh.");
            }

            var result = await _predictService.PredictWithImageAsync(request.Image);

            // Tùy chọn: Bạn có thể kiểm tra xem có chi tiết loài nào được tìm thấy không
            if (result.Prediction == null)
            {
                return NotFound("Không thể dự đoán loài từ hình ảnh hoặc không tìm thấy kết quả.");
            }

            // Trả về đối tượng kết hợp PredictionWithDetail
            return Ok(result);
        }
    }
}