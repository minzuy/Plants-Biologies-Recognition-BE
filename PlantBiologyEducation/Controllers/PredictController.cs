using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlantBiologyEducation.Service;

namespace PlantBiologyEducation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

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

            var resultJson = await _predictService.PredictWithImageAsync(request.Image);

            // Trả về chuỗi JSON trực tiếp
            // ASP.NET Core sẽ tự động thiết lập Content-Type là application/json
            // nếu chuỗi này có định dạng JSON hợp lệ
            return Ok(resultJson);
        }
    }
}