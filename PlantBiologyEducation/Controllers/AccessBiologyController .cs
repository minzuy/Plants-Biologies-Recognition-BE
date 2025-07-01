using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Repository;
using PlantBiologyEducation.Entity.DTO.AccessHistories;

[ApiController]
[Route("api/[controller]")]
public class AccessBiologyController : ControllerBase
{
    private readonly AccessBiologyRepository _repo;
    private readonly Plant_Biology_Animal_Repository _biologies;
    private readonly UserRepository _user;

    public AccessBiologyController(AccessBiologyRepository repo, UserRepository user, Plant_Biology_Animal_Repository biologies)
    {
        _repo = repo;
        _user = user;
        _biologies = biologies;
    }


    // POST: /api/AccessBook/record
    [HttpPost("record")]
    public IActionResult RecordAccess([FromBody] AccessBioRequestDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

         if (!_user.UserExists(dto.User_Id))
            return NotFound("User not existed");

        if (!_biologies.PBAExists(dto.Bio_Id))
            return NotFound("Biology not existed");

        var success = _repo.TrackAccess(dto.User_Id, dto.Bio_Id);
        if (!success)
            return StatusCode(500, "Failed to record access.");

        return Ok(new { message = "Access recorded successfully." });
    }


    [HttpGet("biology/{biologyId}/statistics")]
    public IActionResult GetBiologyStatistics(Guid biologyId)
    {
        var uniqueVisitors = _repo.CountUniqueUsersForBiology(biologyId);
        var visitedNumber = _repo.TotalVisitedNumber(biologyId);

        return Ok(new
        {
            biologyId,
            uniqueVisitors,
            visitedNumber
        });
    }
}
