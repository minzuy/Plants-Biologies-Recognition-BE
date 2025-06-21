using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Repository;
using Plant_BiologyEducation.Entity.Model;
using AutoMapper;
using Plant_BiologyEducation.Entity.DTO.Chapter;

namespace Plant_BiologyEducation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChapterController : ControllerBase
    {
        private readonly ChapterRepository _chapterRepository;
        private readonly BookRepository _bookRepository;
        private readonly IMapper _mapper;

        public ChapterController(ChapterRepository chapterRepository, IMapper mapper, BookRepository bookRepository)
        {
            _chapterRepository = chapterRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchOrGetAllChapter([FromQuery] string? title)
        {
            var chapters = string.IsNullOrWhiteSpace(title)
                ? await _chapterRepository.GetAllChaptersAsync()
                : await _chapterRepository.SearchChapterByTitleAsync(title);

            var chapterDTOs = _mapper.Map<List<ChapterDTO>>(chapters);
            return Ok(chapterDTOs);
        }


        [HttpPost]
        public IActionResult CreateChaptersWithLessons([FromBody] ChapterRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_bookRepository.BookExists(dto.Book_Id))
                return NotFound("Book_Id không tồn tại trong hệ thống.");

            var chapter = _mapper.Map<Chapter>(dto);
            chapter.Chapter_Id = Guid.NewGuid();

            if (chapter.Lessons != null)
            {
                foreach (var lesson in chapter.Lessons)
                {
                    lesson.Lesson_Id = Guid.NewGuid();
                    lesson.Chapter_Id = chapter.Chapter_Id;
                }
            }

            var success = _chapterRepository.CreateChapter(chapter);
            if (!success)
                return StatusCode(500, "An error occurred while saving the chapter.");

            return Ok(new { message = "Chapter created successfully", chapterId = chapter.Chapter_Id });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateChapter(Guid id, [FromBody] ChapterRequestDTO chapterDto)
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

        [HttpDelete("{id}")]
        public IActionResult DeleteChapter(Guid id)
        {
            var chapter = _chapterRepository.GetChapterById(id);
            if (chapter == null)
                return NotFound("Chapter not found.");

            var result = _chapterRepository.DeleteChapter(chapter);
            if (!result)
                return StatusCode(500, "Error deleting the chapter.");

            return Ok("Chapter deleted successfully.");
        }
    }
}
