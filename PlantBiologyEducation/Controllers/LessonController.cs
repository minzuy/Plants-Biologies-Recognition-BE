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

        public LessonController(
            LessonRepository lessonRepository,
            ChapterRepository chapterRepository,
            BookRepository bookRepository,
            IMapper mapper,
            JwtService jwtService)
        {
            _lessonRepository = lessonRepository;
            _chapterRepository = chapterRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
            _jwtService = jwtService;
        }

        [HttpGet("search")]
        public IActionResult SearchOrGetAllLesson([FromQuery] string? title)
        {
            try
            {
                var lessons = string.IsNullOrWhiteSpace(title)
                    ? _lessonRepository.GetAllLessons()
                    : _lessonRepository.SearchLessonsByTitle(title);

                var lessonDTOs = _mapper.Map<List<LessonDTO>>(lessons);
                return Ok(lessonDTOs);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingLesson()
        {
            try
            {
                var pending = await _lessonRepository.GetPendingLessonsAsync();
                var DTOs = _mapper.Map<List<LessonDTO>>(pending);
                return Ok(DTOs);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("chapter/{chapterId}")]
        public async Task<IActionResult> GetLessonByChapterId(Guid chapterId)
        {
            try
            {
                if (!_chapterRepository.ChapterExists(chapterId))
                    return NotFound("Chapter not found.");

                var lessons = await _lessonRepository.GetLessonsByChapterId(chapterId);
                var lessonDTOs = _mapper.Map<List<LessonDTO>>(lessons);
                return Ok(lessonDTOs);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateLesson([FromBody] LessonRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var chapter = _chapterRepository.GetChapterById(dto.Chapter_Id);
                if (chapter == null)
                    return NotFound("Chapter not found.");

                var lesson = _mapper.Map<Lesson>(dto);
                lesson.Lesson_Id = Guid.NewGuid();
                lesson.Status = "Pending";
                lesson.IsActive = false;
                lesson.RejectionReason = null;

                var success = _lessonRepository.CreateLesson(lesson);
                if (!success)
                    return StatusCode(500, "Failed to create lesson.");

                return Ok(new { message = "Lesson created successfully.", lessonId = lesson.Lesson_Id });
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateLesson(Guid id, [FromBody] LessonRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existingLesson = _lessonRepository.GetLessonById(id);
                if (existingLesson == null)
                    return NotFound("Lesson not found.");

                _mapper.Map(dto, existingLesson); // Không cập nhật Chapter_Id

                var result = _lessonRepository.UpdateLesson(existingLesson);
                if (!result)
                    return StatusCode(500, "Error updating lesson.");

                return Ok("Lesson updated successfully.");
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/status")]
        public IActionResult ApproveOrRejectLesson(Guid id, [FromBody] LessonStatusUpdate statusDto)
        {
            try
            {
                var lesson = _lessonRepository.GetLessonById(id);
                if (lesson == null)
                    return NotFound("Lesson not found.");

                var validStatuses = new[] { "Approved", "Rejected" };
                if (!validStatuses.Contains(statusDto.Status))
                    return BadRequest("Invalid status. Must be 'Approved' or 'Rejected'.");

                lesson.Status = statusDto.Status;
                lesson.IsActive = statusDto.Status == "Approved";
                lesson.RejectionReason = statusDto.Status == "Rejected"
                    ? statusDto.RejectionReason ?? "No reason provided"
                    : null;

                var result = _lessonRepository.UpdateLesson(lesson);
                if (!result)
                    return StatusCode(500, "Failed to update lesson status.");

                return Ok(new
                {
                    message = "Lesson status updated.",
                    newStatus = lesson.Status,
                    lessonId = lesson.Lesson_Id
                });
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteLesson(Guid id)
        {
            try
            {
                var lesson = _lessonRepository.GetLessonById(id);
                if (lesson == null)
                    return NotFound("Lesson not found.");

                var result = _lessonRepository.DeleteLesson(lesson);
                if (!result)
                    return StatusCode(500, "Error deleting lesson.");

                return Ok("Lesson deleted successfully.");
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
