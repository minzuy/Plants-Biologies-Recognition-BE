using Microsoft.EntityFrameworkCore;
using Plant_BiologyEducation.Data;
using Plant_BiologyEducation.Entity.Model;

namespace Plant_BiologyEducation.Repository
{
    public class LessonRepository
    {
        private readonly DataContext _context;

        public LessonRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<Lesson?> GetByIdAsync(Guid lessonId)
        {
            return await _context.Lessons
                .Include(l => l.Chapter)
                .ThenInclude(c => c.Book)
                .FirstOrDefaultAsync(l => l.Lesson_Id == lessonId);
        }

        // Lấy tất cả bài học
        public ICollection<Lesson> GetAllLessons()
        {
            return _context.Lessons
                .Include(l => l.Chapter) // include chapter để có dữ liệu liên quan
                .ToList();
        }

        // Tìm bài học theo Id
        public Lesson GetLessonById(Guid id)
        {
            return _context.Lessons
                .Include(l => l.Chapter)
                .FirstOrDefault(l => l.Lesson_Id == id);
        }

        // Tìm bài học theo tên
        public ICollection<Lesson> SearchLessonsByTitle(string title)
        {
            return _context.Lessons
                .Where(l => l.Lesson_Title.Contains(title))
                .Include(l => l.Chapter)
                .OrderBy(l => l.Lesson_Title)
                .ToList();
        }

        // Tạo bài học mới
        public bool CreateLesson(Lesson lesson)
        {
            _context.Lessons.Add(lesson);
            return Save();
        }

        // Cập nhật bài học
        public bool UpdateLesson(Lesson lesson)
        {
            _context.Lessons.Update(lesson);
            return Save();
        }

        // Xóa bài học
        public bool DeleteLesson(Lesson lesson)
        {
            _context.Lessons.Remove(lesson);
            return Save();
        }

        // Kiểm tra bài học có tồn tại không
        public bool LessonExists(Guid id)
        {
            return _context.Lessons.Any(l => l.Lesson_Id == id);
        }

        // Lưu thay đổi
        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
