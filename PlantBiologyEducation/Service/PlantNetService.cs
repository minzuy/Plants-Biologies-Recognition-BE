using Plant_BiologyEducation.Entity.Model.PlanetAPI;
using System.Text.Json;

public class PlantNetService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;

    public PlantNetService(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
    }

    public async Task<PlantNetResponse> IdentifyPlantAsync(IFormFile imageFile)
    {
        var client = _clientFactory.CreateClient("PlantNetClient");
        var apiKey = _configuration["PlantNet:ApiKey"]; // cấu hình ở appsettings.json

        using var content = new MultipartFormDataContent();
        using var stream = imageFile.OpenReadStream();
        content.Add(new StreamContent(stream), "images", imageFile.FileName);

        var response = await client.PostAsync($"identify/all?api-key={apiKey}", content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PlantNetResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}
