using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Entity.DTO.Chapter;
using Plant_BiologyEducation.Entity.DTO.Lesson;
using Plant_BiologyEducation.Entity.Model;
using Plant_BiologyEducation.Repository;
using Plant_BiologyEducation.Service;
using PlantBiologyEducation.Entity.DTO.Lesson;
using System;

namespace Plant_BiologyEducation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly LessonRepository _lessonRepository;
        private readonly ChapterRepository _chapterRepository;
        private readonly BookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly ILogger<LessonController> _logger;

        public LessonController(
            LessonRepository lessonRepository,
            ChapterRepository chapterRepository,
            BookRepository bookRepository,
            IMapper mapper,
            JwtService jwtService,
            ILogger<LessonController> logger)
        {
            _lessonRepository = lessonRepository;
            _chapterRepository = chapterRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpGet("search")]
        public IActionResult SearchOrGetAllLesson([FromQuery] string? title)
        {
            try
            {
                _logger.LogInformation("GET /api/Lesson/search called with title = {Title}", title);

                var lessons = string.IsNullOrWhiteSpace(title)
                    ? _lessonRepository.GetAllLessons()
                    : _lessonRepository.SearchLessonsByTitle(title);

                var lessonDTOs = _mapper.Map<List<LessonDTO>>(lessons);
                return Ok(lessonDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching or retrieving lessons.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingLesson()
        {
            try
            {
                _logger.LogInformation("GET /api/Lesson/pending called");
                var pending = await _lessonRepository.GetPendingLessonsAsync();
                var DTOs = _mapper.Map<List<LessonDTO>>(pending);
                return Ok(DTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting pending lessons.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("chapter/{chapterId}")]
        public async Task<IActionResult> GetLessonByChapterId(Guid chapterId)
        {
            try
            {
                _logger.LogInformation("GET /api/Lesson/chapter/{chapterId} called with id = {Id}", chapterId);

                if (!_chapterRepository.ChapterExists(chapterId))
                {
                    _logger.LogWarning("Chapter not found with id: {Id}", chapterId);
                    return NotFound("Chapter not found.");
                }

                var lessons = await _lessonRepository.GetLessonsByChapterId(chapterId);
                var lessonDTOs = _mapper.Map<List<LessonDTO>>(lessons);
                return Ok(lessonDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting lessons by chapterId: {Id}", chapterId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateLesson([FromBody] LessonRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("POST /api/Lesson called for chapterId: {ChapterId}", dto.Chapter_Id);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var chapter = _chapterRepository.GetChapterById(dto.Chapter_Id);
                if (chapter == null)
                {
                    _logger.LogWarning("Chapter not found for lesson creation with id: {Id}", dto.Chapter_Id);
                    return NotFound("Chapter not found.");
                }

                var lesson = _mapper.Map<Lesson>(dto);
                lesson.Lesson_Id = Guid.NewGuid();
                lesson.Status = "Pending";
                lesson.IsActive = false;
                lesson.RejectionReason = null;

                var success = _lessonRepository.CreateLesson(lesson);
                if (!success)
                {
                    _logger.LogError("Failed to create lesson for chapterId: {ChapterId}", dto.Chapter_Id);
                    return StatusCode(500, "Failed to create lesson.");
                }

                _logger.LogInformation("Lesson created successfully with id: {LessonId}", lesson.Lesson_Id);
                return Ok(new { message = "Lesson created successfully.", lessonId = lesson.Lesson_Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while creating lesson.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateLesson(Guid id, [FromBody] LessonRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("PUT /api/Lesson/{id} called with id: {Id}", id);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existingLesson = _lessonRepository.GetLessonById(id);
                if (existingLesson == null)
                {
                    _logger.LogWarning("Lesson not found with id: {Id}", id);
                    return NotFound("Lesson not found.");
                }

                _mapper.Map(dto, existingLesson); // Không cập nhật Chapter_Id

                var result = _lessonRepository.UpdateLesson(existingLesson);
                if (!result)
                {
                    _logger.LogError("Failed to update lesson with id: {Id}", id);
                    return StatusCode(500, "Error updating lesson.");
                }

                _logger.LogInformation("Lesson updated successfully with id: {Id}", id);
                return Ok("Lesson updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while updating lesson with id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/status")]
        public IActionResult ApproveOrRejectLesson(Guid id, [FromBody] LessonStatusUpdate statusDto)
        {
            try
            {
                _logger.LogInformation("PUT /api/Lesson/{id}/status called with id: {Id}, status: {Status}", id, statusDto.Status);

                var lesson = _lessonRepository.GetLessonById(id);
                if (lesson == null)
                {
                    _logger.LogWarning("Lesson not found with id: {Id}", id);
                    return NotFound("Lesson not found.");
                }

                var validStatuses = new[] { "Approved", "Rejected" };
                if (!validStatuses.Contains(statusDto.Status))
                {
                    _logger.LogWarning("Invalid status {Status} for lesson id: {Id}", statusDto.Status, id);
                    return BadRequest("Invalid status. Must be 'Approved' or 'Rejected'.");
                }

                lesson.Status = statusDto.Status;
                lesson.IsActive = statusDto.Status == "Approved";
                lesson.RejectionReason = statusDto.Status == "Rejected"
                    ? statusDto.RejectionReason ?? "No reason provided"
                    : null;

                var result = _lessonRepository.UpdateLesson(lesson);
                if (!result)
                {
                    _logger.LogError("Failed to update lesson status with id: {Id}", id);
                    return StatusCode(500, "Failed to update lesson status.");
                }

                _logger.LogInformation("Lesson status updated to {Status} for lesson id: {Id}", statusDto.Status, id);
                return Ok(new
                {
                    message = "Lesson status updated.",
                    newStatus = lesson.Status,
                    lessonId = lesson.Lesson_Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while updating lesson status with id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteLesson(Guid id)
        {
            try
            {
                _logger.LogInformation("DELETE /api/Lesson/{id} called with id: {Id}", id);
                var lesson = _lessonRepository.GetLessonById(id);
                if (lesson == null)
                {
                    _logger.LogWarning("Lesson not found with id: {Id}", id);
                    return NotFound("Lesson not found.");
                }

                var result = _lessonRepository.DeleteLesson(lesson);
                if (!result)
                {
                    _logger.LogError("Error deleting lesson with id: {Id}", id);
                    return StatusCode(500, "Error deleting lesson.");
                }

                _logger.LogInformation("Lesson deleted successfully with id: {Id}", id);
                return Ok("Lesson deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while deleting lesson with id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
