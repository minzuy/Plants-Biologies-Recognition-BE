using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Entity.DTO.Management;
using Plant_BiologyEducation.Entity.Model;
using Plant_BiologyEducation.Repository;

namespace Plant_BiologyEducation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ManageChapterController : ControllerBase
    {
        private readonly ChapterRepository _chapterRepository;
        private readonly ManageBookRepository _manageBookRepository;
        private readonly ManageChapterRepository _manageChapterRepository;

        public ManageChapterController(
            ChapterRepository chapterRepository,
            ManageBookRepository manageBookRepository,
            ManageChapterRepository manageChapterRepository)
        {
            _chapterRepository = chapterRepository;
            _manageBookRepository = manageBookRepository;
            _manageChapterRepository = manageChapterRepository;
        }

        [HttpPost("update")]
        public async Task<IActionResult> TrackChapterEdit([FromBody] ManageChapterDTO dto)
        {
            // Ghi nhận chỉnh sửa chapter
            var result = await _manageChapterRepository.TrackChapterEditAsync(dto.User_Id, dto.Chapter_Id);

            // Lấy chapter để truy xuất Book_Id
            var chapter = await _chapterRepository.GetByIdAsync(dto.Chapter_Id);
            var bookId = chapter?.Book?.Book_Id;

            if (bookId != null)
            {
                await _manageBookRepository.TrackBookEditAsync(dto.User_Id, bookId.Value);
            }

            return Ok(new { success = result });
        }
    }
}
