using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Entity.DTO.AccessHistories;
using Plant_BiologyEducation.Repository;
using PlantBiologyEducation.Entity.DTO.AccessHistories;

[ApiController]
[Route("api/[controller]")]
public class AccessLessonController : ControllerBase
{
    private readonly AccessLessonRepository _repo;
    private readonly UserRepository _user;
    private readonly LessonRepository _lesson;
    public AccessLessonController(AccessLessonRepository repo, LessonRepository lesson,UserRepository user)
    {
        _repo = repo;
        _lesson = lesson;
        _user = user;

    }
    // POST: /api/AccessBook/record
    [HttpPost("record")]
    public IActionResult RecordAccess([FromBody] AccessLessonRequestDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        if (!_user.UserExists(dto.User_Id))
            return NotFound("User not existed");

        if (!_lesson.LessonExists(dto.Lesson_Id))
            return NotFound("Lesson not existed");

        var success = _repo.TrackAccess(dto.User_Id, dto.Lesson_Id);
        if (!success)
            return StatusCode(500, "Failed to record access.");

        return Ok(new { message = "Access recorded successfully." });
    }


    [HttpGet("lesson/{lessonId}/statistics")]
    public IActionResult GetLessonStatistics(Guid lessonId)
    {
        var uniqueVisitors = _repo.CountUniqueUsersForLesson(lessonId);
        var visitedNumber = _repo.TotalVisitedNumber(lessonId);

        return Ok(new
        {
            lessonId,
            uniqueVisitors,
            visitedNumber
        });
    }
}
