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
        private readonly ILogger<ChapterController> _logger;

        public ChapterController(
            ChapterRepository chapterRepository,
            IMapper mapper,
            BookRepository bookRepository,
            ILogger<ChapterController> logger)
        {
            _chapterRepository = chapterRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchOrGetAllChapter([FromQuery] string? title)
        {
            try
            {
                _logger.LogInformation("GET /api/Chapter/search called with title = {title}", title);

                var chapters = string.IsNullOrWhiteSpace(title)
                    ? await _chapterRepository.GetAllChaptersAsync()
                    : await _chapterRepository.SearchChapterByTitleAsync(title);

                var chapterDTOs = _mapper.Map<List<ChapterDTO>>(chapters);
                return Ok(chapterDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching or fetching all chapters.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingChapters()
        {
            try
            {
                _logger.LogInformation("GET /api/Chapter/pending called.");
                var pendingChapters = await _chapterRepository.GetPendingChaptersAsync();
                var chapterDTOs = _mapper.Map<List<ChapterDTO>>(pendingChapters);
                return Ok(chapterDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending chapters.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChapterById(Guid id)
        {
            try
            {
                _logger.LogInformation("GET /api/Chapter/{id} called with id = {Id}", id);
                var chapter = await _chapterRepository.GetByIdAsync(id);
                if (chapter == null)
                {
                    _logger.LogWarning("Chapter not found with id: {Id}", id);
                    return NotFound("Chapter not found.");
                }

                var chapterDto = _mapper.Map<ChapterDTO>(chapter);
                return Ok(chapterDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting chapter by id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetChaptersByBookId(Guid bookId)
        {
            try
            {
                _logger.LogInformation("GET /api/Chapter/book/{bookId} called with bookId = {BookId}", bookId);
                if (!_bookRepository.BookExists(bookId))
                {
                    _logger.LogWarning("Book not found with id: {BookId}", bookId);
                    return NotFound("Book not found.");
                }

                var chapters = await _chapterRepository.GetChaptersByBookIdAsync(bookId);
                var chapterDTOs = _mapper.Map<List<ChapterDTO>>(chapters);
                return Ok(chapterDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching chapters by bookId: {BookId}", bookId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateChaptersWithLessons([FromBody] ChapterRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("POST /api/Chapter called to create new chapter for bookId = {BookId}", dto.Book_Id);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_bookRepository.BookExists(dto.Book_Id))
                {
                    _logger.LogWarning("Book not found with id: {BookId}", dto.Book_Id);
                    return NotFound("Book not found");
                }

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
                {
                    _logger.LogError("Failed to save chapter for bookId: {BookId}", dto.Book_Id);
                    return StatusCode(500, "An error occurred while saving the chapter.");
                }

                _logger.LogInformation("Chapter created successfully with id: {ChapterId}", chapter.Chapter_Id);
                return Ok(new { message = "Chapter created successfully", chapterId = chapter.Chapter_Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating chapter for bookId: {BookId}", dto.Book_Id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateChapter(Guid id, [FromBody] ChapterRequestDTO chapterDto)
        {
            try
            {
                _logger.LogInformation("PUT /api/Chapter/{id} called with id: {Id}", id);

                if (!_chapterRepository.ChapterExists(id))
                {
                    _logger.LogWarning("Chapter not found with id: {Id}", id);
                    return NotFound("Chapter not found.");
                }

                var chapter = _mapper.Map<Chapter>(chapterDto);
                chapter.Chapter_Id = id;

                var result = _chapterRepository.UpdateChapter(chapter);
                if (!result)
                {
                    _logger.LogError("Error updating chapter with id: {Id}", id);
                    return StatusCode(500, "Error updating the chapter.");
                }

                _logger.LogInformation("Chapter updated successfully with id: {Id}", id);
                return Ok("Chapter updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating chapter with id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/status")]
        public IActionResult ApproveOrRejectChapter(Guid id, [FromBody] ChapterStatusUpdate statusDto)
        {
            try
            {
                _logger.LogInformation("PUT /api/Chapter/{id}/status called with id: {Id} and status: {Status}", id, statusDto.Status);

                var chapter = _chapterRepository.GetChapterById(id);
                if (chapter == null)
                {
                    _logger.LogWarning("Chapter not found with id: {Id}", id);
                    return NotFound("Chapter not found.");
                }

                var validStatuses = new[] { "Approved", "Rejected" };
                if (!validStatuses.Contains(statusDto.Status))
                {
                    _logger.LogWarning("Invalid status: {Status} provided for chapter: {Id}", statusDto.Status, id);
                    return BadRequest("Invalid status. Must be 'Approved' or 'Rejected'.");
                }

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
                {
                    _logger.LogError("Failed to update chapter status with id: {Id}", id);
                    return StatusCode(500, "Failed to update chapter status.");
                }

                _logger.LogInformation("Chapter status updated to {Status} for chapter id: {Id}", statusDto.Status, id);
                return Ok(new
                {
                    message = "Chapter status updated.",
                    newStatus = chapter.Status,
                    chapterId = chapter.Chapter_Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while updating chapter status with id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteChapter(Guid id)
        {
            try
            {
                _logger.LogInformation("DELETE /api/Chapter/{id} called with id: {Id}", id);

                var chapter = _chapterRepository.GetChapterById(id);
                if (chapter == null)
                {
                    _logger.LogWarning("Chapter not found with id: {Id}", id);
                    return NotFound("Chapter not found.");
                }

                var result = _chapterRepository.DeleteChapter(chapter);
                if (!result)
                {
                    _logger.LogError("Error deleting chapter with id: {Id}", id);
                    return StatusCode(500, "Error deleting the chapter.");
                }

                _logger.LogInformation("Chapter deleted successfully with id: {Id}", id);
                return Ok("Chapter deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting chapter with id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
