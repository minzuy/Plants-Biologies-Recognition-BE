using Microsoft.AspNetCore.Mvc;
using PlantBiologyEducation.ObjectDetections;
using System.Text.Json;

namespace Plant_BiologyEducation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ObjectDetectionController : ControllerBase
    {
        private readonly ObjectDetection _detector;
        private readonly Dictionary<string, JsonElement> _speciesDetails;

        public ObjectDetectionController()
        {
            var jsonPath = Path.Combine("Service", "DetailSpecies.json");
            var jsonText = System.IO.File.ReadAllText(jsonPath);
            _speciesDetails = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonText)!;

            Console.WriteLine("=== Loaded Labels from DetailSpecies.json ===");
            foreach (var label in _speciesDetails.Keys)
            {
                Console.WriteLine(label);
            }

            var labels = _speciesDetails.Keys.ToList();
            _detector = new ObjectDetection("Service/best.onnx", labels);
        }

        [HttpPost("predict")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Predict([FromForm] UploadImgDTO dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("Vui lòng upload ảnh.");

            try
            {
                using var ms = new MemoryStream();
                await dto.File.CopyToAsync(ms);
                var imageBytes = ms.ToArray();

                var (imageResult, predictions) = _detector.Predict(imageBytes, confidenceThreshold: 0.7f);

                if (predictions.Count == 0)
                {
                    return Ok(new
                    {
                        Message = "Không phát hiện được đối tượng nào trong ảnh với độ tin cậy đủ cao.",
                        Results = new List<object>(),
                        ImageBase64 = Convert.ToBase64String(imageResult)
                    });
                }

                var resultList = new List<object>();

                foreach (var p in predictions)
                {
                    JsonElement? details = null;
                    if (_speciesDetails.TryGetValue(p.Label, out var detail))
                    {
                        details = detail;
                    }

                    resultList.Add(new
                    {
                        Label = p.Label,
                        Confidence = Math.Round(p.Confidence * 100, 2),
                        BoundingBox = new
                        {
                            X = Math.Round(p.X, 2),
                            Y = Math.Round(p.Y, 2),
                            Width = Math.Round(p.Width, 2),
                            Height = Math.Round(p.Height, 2)
                        },
                        Details = details
                    });
                }

                var base64Image = Convert.ToBase64String(imageResult);

                return Ok(new
                {
                    Message = $"Phát hiện {predictions.Count} đối tượng",
                    Results = resultList,
                    ImageBase64 = base64Image
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Có lỗi xảy ra khi xử lý ảnh",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("labels")]
        public IActionResult GetLabels()
        {
            return Ok(new
            {
                Labels = _speciesDetails.Keys.ToList(),
                Count = _speciesDetails.Count
            });
        }

        [HttpGet("species/{name}")]
        public IActionResult GetSpeciesDetails(string name)
        {
            if (_speciesDetails.TryGetValue(name, out var details))
            {
                return Ok(details);
            }
            return NotFound($"Không tìm thấy thông tin về loài: {name}");
        }
    }

    public class UploadImgDTO
    {
        public IFormFile File { get; set; }
    }
}
