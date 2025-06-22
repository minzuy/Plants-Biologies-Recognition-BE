using Microsoft.Extensions.Options;
using Plant_BiologyEducation.Entity.Model.MyPlant;
using System.Text.Json;

namespace Plant_BiologyEducation.Services
{
    public class MyPlanetService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly ILogger<MyPlanetService> _logger;

        public MyPlanetService(IOptions<MyPlanetModel> settings, HttpClient httpClient, ILogger<MyPlanetService> logger)
        {
            _apiKey = settings.Value.ApiKey;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PlantIdentificationResult> IdentifyPlantAsync(IFormFile imageFile, string project = "all", string organ = "leaf")
        {
            try
            {
                var apiUrl = $"https://my-api.plantnet.org/v1/identify/{project}?api-key={_apiKey}";
                _logger.LogInformation($"Calling PlantNet API: {apiUrl}");

                using var content = new MultipartFormDataContent();

                // Thêm file ảnh vào request
                using var fileStream = imageFile.OpenReadStream();
                using var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);
                content.Add(streamContent, "images", imageFile.FileName);

                // Thêm các parameters theo PlantNet API
                content.Add(new StringContent(organ), "organs");
                content.Add(new StringContent("true"), "include-related-images");

                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"API Response: {jsonResponse}");

                    var result = JsonSerializer.Deserialize<PlantApiResponse>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    return MapToPlantIdentificationResult(result);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"API call failed with status: {response.StatusCode}, Error: {errorContent}");
                    throw new Exception($"API call failed with status: {response.StatusCode}, Error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error identifying plant");
                throw new Exception($"Error identifying plant: {ex.Message}", ex);
            }
        }

        private PlantIdentificationResult MapToPlantIdentificationResult(PlantApiResponse apiResponse)
        {
            var topResult = apiResponse.Results?.FirstOrDefault();

            return new PlantIdentificationResult
            {
                IsSuccess = apiResponse.Results?.Any() == true,
                PlantName = topResult?.Species?.Genus?.ScientificNameWithoutAuthor ?? topResult?.Species?.ScientificNameWithoutAuthor,
                ScientificName = $"{topResult?.Species?.ScientificNameWithoutAuthor} {topResult?.Species?.ScientificNameAuthorship}".Trim(),
                Probability = topResult?.Score ?? 0,
                Suggestions = apiResponse.Results?.Select(r => new PlantSuggestion
                {
                    PlantName = r.Species?.Genus?.ScientificNameWithoutAuthor ?? r.Species?.ScientificNameWithoutAuthor,
                    ScientificName = $"{r.Species?.ScientificNameWithoutAuthor} {r.Species?.ScientificNameAuthorship}".Trim(),
                    Probability = r.Score,
                    CommonNames = r.Species?.CommonNames?.Select(cn => cn.Name).ToList() ?? new List<string>()
                }).ToList() ?? new List<PlantSuggestion>(),
                SimilarImages = topResult?.Images?.Select(img => new SimilarImage
                {
                    Id = img.Author,
                    Url = img.Url?.M ?? img.Url?.S ?? img.Url?.O,
                    Similarity = 1.0
                }).ToList() ?? new List<SimilarImage>(),
                RemainingRequests = apiResponse.RemainingIdentificationRequests
            };
        }
    }
}