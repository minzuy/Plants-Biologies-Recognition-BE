using Microsoft.EntityFrameworkCore;
using Plant_BiologyEducation.Data;
using Plant_BiologyEducation.Entity.Model;

namespace Plant_BiologyEducation.Repository
{
    public class ChapterRepository
    {
        private readonly DataContext _context;

        public ChapterRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<Chapter?> GetByIdAsync(Guid chapterId)
        {
            return await _context.Chapters
                .Include(c => c.Book) // nếu bạn cần truy xuất Book_Id
                .FirstOrDefaultAsync(c => c.Chapter_Id == chapterId);
        }

        // Lấy toàn bộ Chapter
        public async Task<List<Chapter>> GetAllChaptersAsync()
        {
            return await _context.Chapters
                .Include(c => c.Book)
                .Include(c => c.Lessons)
                .ToListAsync();
        }

        // Tìm kiếm Chapter theo tiêu đề
        public async Task<List<Chapter>> SearchChapterByTitleAsync(string title)
        {
            return await _context.Chapters
                .Where(c => c.Chapter_Title.Contains(title))
                .Include(c => c.Book)
                .Include(c => c.Lessons)
                .ToListAsync();
        }


        // Thêm mới Chapter
        public bool CreateChapter(Chapter chapter)
        {
            _context.Chapters.Add(chapter);
            return Save();
        }

        // Cập nhật Chapter
        public bool UpdateChapter(Chapter chapter)
        {
            _context.Chapters.Update(chapter);
            return Save();
        }

        // Xoá Chapter
        public bool DeleteChapter(Chapter chapter)
        {
            _context.Chapters.Remove(chapter);
            return Save();
        }

        // Lấy Chapter theo Id (nếu cần cho edit UI)
        public Chapter GetChapterById(Guid id)
        {
            return _context.Chapters
                .Include(c => c.Lessons)
                .FirstOrDefault(c => c.Chapter_Id == id);
        }

        // Kiểm tra Chapter có tồn tại không
        public bool ChapterExists(Guid id)
        {
            return _context.Chapters.Any(c => c.Chapter_Id == id);
        }

        // Lưu thay đổi
        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
