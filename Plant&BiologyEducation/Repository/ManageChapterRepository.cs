using Microsoft.EntityFrameworkCore;
using Plant_BiologyEducation.Data;
using Plant_BiologyEducation.Entity.Model;

namespace Plant_BiologyEducation.Repository
{
    public class ManageChapterRepository
    {
        private readonly DataContext _context;

        public ManageChapterRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> TrackChapterEditAsync(Guid userId, Guid chapterId)
        {
            // Tìm Chapter bao gồm cả Book và danh sách người quản lý Book
            var chapter = await _context.Chapters
                .Include(c => c.Book)
                    .ThenInclude(b => b.ManagedBy)
                .FirstOrDefaultAsync(c => c.Chapter_Id == chapterId);

            if (chapter == null || chapter.Book == null)
                return false;

            // 1. Ghi đè hoặc thêm mới bản ghi ManageChapter
            var existingManageChapter = await _context.ManageChapters
                .FirstOrDefaultAsync(x => x.User_Id == userId && x.Chapter_Id == chapterId);

            if (existingManageChapter != null)
            {
                existingManageChapter.UpdatedDate = DateTime.UtcNow;
            }
            else
            {
                await _context.ManageChapters.AddAsync(new ManageChapter
                {
                    User_Id = userId,
                    Chapter_Id = chapterId,
                    UpdatedDate = DateTime.UtcNow
                });
            }

            // 2. Nếu user cũng quản lý Book thì cập nhật UpdatedDate trong ManageBook
            foreach (var manageBook in chapter.Book.ManagedBy
                .Where(mb => mb.User_Id == userId))
            {
                manageBook.UpdatedDate = DateTime.UtcNow;
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
