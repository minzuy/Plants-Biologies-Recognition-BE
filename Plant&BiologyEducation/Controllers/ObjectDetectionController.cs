using Microsoft.AspNetCore.Mvc;
using PlantBiologyEducation.ObjectDetections;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class ObjectDetectionController : ControllerBase
{
    private readonly ObjectDetection _detector;
    private readonly Dictionary<string, string> _speciesDetails;

    public ObjectDetectionController()
    {
        _detector = new ObjectDetection("Service/best.onnx");

        // Load từ JSON mapping
        var jsonPath = Path.Combine("Service", "DetailSpecies.json");
        var jsonText = System.IO.File.ReadAllText(jsonPath);
        _speciesDetails = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonText)!;
    }

    [HttpPost("predict")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Predict([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Vui lòng upload ảnh.");

        using var stream = file.OpenReadStream();
        using var image = Image.FromStream(stream);
        using var bitmap = new Bitmap(image);

        var resultBitmap = _detector.Predict(bitmap, out var predictions);

        var resultList = predictions.Select(p => new
        {
            Label = p.Label,
            Confidence = p.Confidence,
            Description = _speciesDetails.ContainsKey(p.Label) ? _speciesDetails[p.Label] : "Không có mô tả"
        });

        // Chuyển ảnh kết quả sang base64 để trả kèm
        using var ms = new MemoryStream();
        resultBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
        var base64Image = Convert.ToBase64String(ms.ToArray());

        return Ok(new
        {
            Results = resultList,
            ImageBase64 = base64Image
        });
    }
}
