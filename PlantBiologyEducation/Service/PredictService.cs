
using System.Net.Http.Headers;
using System.Text.Json;

namespace PlantBiologyEducation.Service
{
    public class PredictService
    {
        private readonly HttpClient _httpClient;

        public PredictService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PredictAPI");
        }

        public async Task<string> PredictWithImageAsync(IFormFile imageFile)
        {
            using var content = new MultipartFormDataContent();

            using var fileStream = imageFile.OpenReadStream();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);

            content.Add(streamContent, "image", imageFile.FileName);

            var response = await _httpClient.PostAsync("https://plant-ai-api.onrender.com/predict", content);
            response.EnsureSuccessStatusCode();

            var rawJsonResponse = await response.Content.ReadAsStringAsync();

            string actualJsonContent;
            try
            {
                // Thử deserialize thành string để loại bỏ các ký tự thoát và dấu ngoặc kép ngoài cùng
                actualJsonContent = JsonSerializer.Deserialize<string>(rawJsonResponse);

                // Kiểm tra lại nếu chuỗi sau khi unescape không phải là JSON hợp lệ
                if (!actualJsonContent.TrimStart().StartsWith("[") && !actualJsonContent.TrimStart().StartsWith("{"))
                {
                    actualJsonContent = rawJsonResponse; // Giữ nguyên bản nếu không phải JSON escaped
                }
            }
            catch (JsonException)
            {
                // Nếu không thể deserialize thành string, có thể rawJsonResponse đã là JSON nguyên bản
                actualJsonContent = rawJsonResponse;
            }

            return actualJsonContent;
        }
    }
}