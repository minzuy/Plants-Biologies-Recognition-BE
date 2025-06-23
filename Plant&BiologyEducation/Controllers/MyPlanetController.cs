using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class PlantNetIdentifyController : ControllerBase
{
    private readonly PlantNetService _plantNetService;

    public PlantNetIdentifyController(PlantNetService plantNetService)
    {
        _plantNetService = plantNetService;
    }

    [HttpPost("identify")]
    public async Task<IActionResult> IdentifyPlant(IFormFile image)
    {
        if (image == null || image.Length == 0)
            return BadRequest("No image uploaded.");

        var result = await _plantNetService.IdentifyPlantAsync(image);
        return Ok(result);
    }
}
