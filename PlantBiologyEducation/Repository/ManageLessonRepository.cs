using Microsoft.EntityFrameworkCore;
using Plant_BiologyEducation.Data;
using Plant_BiologyEducation.Entity.Model;

namespace Plant_BiologyEducation.Repository
{
    public class ManageLessonRepository
    {
        private readonly DataContext _context;

        public ManageLessonRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> TrackLessonEditAsync(Guid userId, Guid lessonId)
        {
            // Tìm lesson bao gồm cả Chapter và Book
            var lesson = await _context.Lessons
                .Include(l => l.Chapter)
                    .ThenInclude(c => c.Book)
                        .ThenInclude(b => b.ManagedBy) // lấy danh sách người quản lý Book
                .FirstOrDefaultAsync(l => l.Lesson_Id == lessonId);

            if (lesson == null || lesson.Chapter == null || lesson.Chapter.Book == null)
                return false;

            // 1. Ghi đè hoặc thêm mới bản ghi ManageLesson
            var existingManageLesson = await _context.ManageLessons
                .FirstOrDefaultAsync(x => x.User_Id == userId && x.Lesson_Id == lessonId);

            if (existingManageLesson != null)
            {
                existingManageLesson.UpdatedDate = DateTime.UtcNow;
            }
            else
            {
                await _context.ManageLessons.AddAsync(new ManageLesson
                {
                    User_Id = userId,
                    Lesson_Id = lessonId,
                    UpdatedDate = DateTime.UtcNow
                });
            }

            // 2. Nếu user đó cũng quản lý Book thì cập nhật UpdatedDate trong ManageBook
            foreach (var manageBook in lesson.Chapter.Book.ManagedBy
                .Where(mb => mb.User_Id == userId))
            {
                manageBook.UpdatedDate = DateTime.UtcNow;
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
