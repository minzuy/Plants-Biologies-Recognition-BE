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
    public class ManageLessonController : ControllerBase
    {
        private readonly LessonRepository _lessonRepository;
        private readonly ManageBookRepository _manageBookRepository;
        private readonly ManageLessonRepository _manageLessonRepository;

        public ManageLessonController(
            LessonRepository lessonRepository,
            ManageBookRepository manageBookRepository,
            ManageLessonRepository manageLessonRepository)
        {
            _lessonRepository = lessonRepository;
            _manageBookRepository = manageBookRepository;
            _manageLessonRepository = manageLessonRepository;
        }

        [HttpPost("update")]
        public async Task<IActionResult> TrackLessonEdit([FromBody] ManageLessonDTO dto)
        {
            // Ghi nhận chỉnh sửa bài học (có kiểm tra trùng, ghi đè nếu đã tồn tại)
            var result = await _manageLessonRepository.TrackLessonEditAsync(dto.User_Id, dto.Lesson_Id);

            // Lấy lesson để truy xuất Book_Id thông qua Chapter
            var lesson = await _lessonRepository.GetByIdAsync(dto.Lesson_Id);
            var bookId = lesson?.Chapter?.Book_Id;

            if (bookId != null)
            {
                await _manageBookRepository.TrackBookEditAsync(dto.User_Id, bookId.Value);
            }

            return Ok(new { success = result });
        }
    }
}
