
using PlantBiologyEducation.Entity.Model.Training;
using System.Net.Http.Headers;
using System.Text.Json;
namespace PlantBiologyEducation.Service
{ 
public class PredictService
{
    private readonly HttpClient _httpClient;
    private readonly IDictionary<string, SpeciesDetail> _speciesDetails;
    private readonly IHostEnvironment _hostEnvironment; // Inject IHostEnvironment

    public PredictService(IHttpClientFactory httpClientFactory, IHostEnvironment hostEnvironment)
    {
        _httpClient = httpClientFactory.CreateClient("PredictAPI");
        _hostEnvironment = hostEnvironment;
        _speciesDetails = LoadSpeciesDetails(Path.Combine(_hostEnvironment.ContentRootPath, "DetailSpecies.json"));
    }

    private IDictionary<string, SpeciesDetail> LoadSpeciesDetails(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: DetailSpecies.json not found at {filePath}");
                return new Dictionary<string, SpeciesDetail>();
            }

            string jsonString = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<Dictionary<string, SpeciesDetail>>(jsonString, options);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error deserializing DetailSpecies.json: {ex.Message}");
            return new Dictionary<string, SpeciesDetail>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred while loading DetailSpecies.json: {ex.Message}");
            return new Dictionary<string, SpeciesDetail>();
        }
    }

    public async Task<PredictionWithDetail> PredictWithImageAsync(IFormFile imageFile)
    {
        using var content = new MultipartFormDataContent();

        using var fileStream = imageFile.OpenReadStream();
        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);

        content.Add(streamContent, "image", imageFile.FileName);

        var response = await _httpClient.PostAsync("https://plant-ai-api.onrender.com/predict", content);
        response.EnsureSuccessStatusCode();

        var rawJsonResponse = await response.Content.ReadAsStringAsync();

        // Xử lý chuỗi JSON bị mã hóa (escaped JSON)
        string actualJsonContent;
        try
        {
            // Thử deserialize thành string để loại bỏ các ký tự thoát và dấu ngoặc kép ngoài cùng
            actualJsonContent = JsonSerializer.Deserialize<string>(rawJsonResponse);

            // Kiểm tra lại nếu chuỗi sau khi unescape không phải là JSON hợp lệ (không bắt đầu bằng '[' hoặc '{')
            // Điều này có thể xảy ra nếu API trả về một chuỗi không phải JSON escaped.
            if (!actualJsonContent.Trim().StartsWith("[") && !actualJsonContent.Trim().StartsWith("{"))
            {
                actualJsonContent = rawJsonResponse; // Giữ nguyên bản nếu không phải JSON escaped
            }
        }
        catch (JsonException)
        {
            // Nếu không thể deserialize thành string, có thể rawJsonResponse đã là JSON nguyên bản
            actualJsonContent = rawJsonResponse;
        }

        // Deserialize chuỗi JSON đã được giải mã thành List<PredictResult>
        var predictResults = JsonSerializer.Deserialize<List<PredictResult>>(actualJsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var predictedSpeciesName = predictResults?.FirstOrDefault()?.Name;

        PredictionWithDetail result = new PredictionWithDetail
        {
            Prediction = predictResults?.FirstOrDefault() // Gán kết quả dự đoán
        };

        if (predictedSpeciesName != null && _speciesDetails.TryGetValue(predictedSpeciesName, out var speciesDetail))
        {
            result.Detail = speciesDetail; // Gán thông tin chi tiết loài
        }

        return result; // Trả về đối tượng kết hợp
    }
}
}
