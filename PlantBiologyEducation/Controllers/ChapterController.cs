using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Repository;
using Plant_BiologyEducation.Entity.Model;
using AutoMapper;
using Plant_BiologyEducation.Entity.DTO.Chapter;
using PlantBiologyEducation.Entity.DTO.Chapter;

namespace Plant_BiologyEducation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChapterController : ControllerBase
    {
        private readonly ChapterRepository _chapterRepository;
        private readonly BookRepository _bookRepository;
        private readonly IMapper _mapper;

        public ChapterController(
            ChapterRepository chapterRepository,
            IMapper mapper,
            BookRepository bookRepository)
        {
            _chapterRepository = chapterRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchOrGetAllChapter([FromQuery] string? title)
        {
            try
            {
                var chapters = string.IsNullOrWhiteSpace(title)
                    ? await _chapterRepository.GetAllChaptersAsync()
                    : await _chapterRepository.SearchChapterByTitleAsync(title);

                var chapterDTOs = _mapper.Map<List<ChapterDTO>>(chapters);
                return Ok(chapterDTOs);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingChapters()
        {
            try
            {
                var pendingChapters = await _chapterRepository.GetPendingChaptersAsync();
                var chapterDTOs = _mapper.Map<List<ChapterDTO>>(pendingChapters);
                return Ok(chapterDTOs);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChapterById(Guid id)
        {
            try
            {
                var chapter = await _chapterRepository.GetByIdAsync(id);
                if (chapter == null)
                    return NotFound("Chapter not found.");

                var chapterDto = _mapper.Map<ChapterDTO>(chapter);
                return Ok(chapterDto);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetChaptersByBookId(Guid bookId)
        {
            try
            {
                if (!_bookRepository.BookExists(bookId))
                    return NotFound("Book not found.");

                var chapters = await _chapterRepository.GetChaptersByBookIdAsync(bookId);
                var chapterDTOs = _mapper.Map<List<ChapterDTO>>(chapters);
                return Ok(chapterDTOs);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateChaptersWithLessons([FromBody] ChapterRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_bookRepository.BookExists(dto.Book_Id))
                    return NotFound("Book not found");

                var chapter = _mapper.Map<Chapter>(dto);
                chapter.Chapter_Id = Guid.NewGuid();
                chapter.Status = "Pending";
                chapter.IsActive = false;
                chapter.RejectionReason = null;

                if (chapter.Lessons != null)
                {
                    foreach (var lesson in chapter.Lessons)
                    {
                        lesson.Lesson_Id = Guid.NewGuid();
                        lesson.Chapter_Id = chapter.Chapter_Id;
                        lesson.Status = "Pending";
                        lesson.IsActive = false;
                        lesson.RejectionReason = null;
                    }
                }

                var success = _chapterRepository.CreateChapter(chapter);
                if (!success)
                    return StatusCode(500, "An error occurred while saving the chapter.");

                return Ok(new { message = "Chapter created successfully", chapterId = chapter.Chapter_Id });
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateChapter(Guid id, [FromBody] ChapterRequestDTO chapterDto)
        {
            try
            {
                if (!_chapterRepository.ChapterExists(id))
                    return NotFound("Chapter not found.");

                var chapter = _mapper.Map<Chapter>(chapterDto);
                chapter.Chapter_Id = id;

                var result = _chapterRepository.UpdateChapter(chapter);
                if (!result)
                    return StatusCode(500, "Error updating the chapter.");

                return Ok("Chapter updated successfully.");
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/status")]
        public IActionResult ApproveOrRejectChapter(Guid id, [FromBody] ChapterStatusUpdate statusDto)
        {
            try
            {
                var chapter = _chapterRepository.GetChapterById(id);
                if (chapter == null)
                    return NotFound("Chapter not found.");

                var validStatuses = new[] { "Approved", "Rejected" };
                if (!validStatuses.Contains(statusDto.Status))
                    return BadRequest("Invalid status. Must be 'Approved' or 'Rejected'.");

                chapter.Status = statusDto.Status;

                if (statusDto.Status == "Rejected")
                {
                    chapter.RejectionReason = statusDto.RejectionReason ?? "No reason provided";
                    chapter.IsActive = false;
                }
                else if (statusDto.Status == "Approved")
                {
                    chapter.RejectionReason = null;
                    chapter.IsActive = true;
                }

                var result = _chapterRepository.UpdateChapter(chapter);
                if (!result)
                    return StatusCode(500, "Failed to update chapter status.");

                return Ok(new
                {
                    message = "Chapter status updated.",
                    newStatus = chapter.Status,
                    chapterId = chapter.Chapter_Id
                });
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteChapter(Guid id)
        {
            try
            {
                var chapter = _chapterRepository.GetChapterById(id);
                if (chapter == null)
                    return NotFound("Chapter not found.");

                var result = _chapterRepository.DeleteChapter(chapter);
                if (!result)
                    return StatusCode(500, "Error deleting the chapter.");

                return Ok("Chapter deleted successfully.");
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
