using PlantBiologyEducation.Entity.Model;
using Plant_BiologyEducation.Data;

namespace Plant_BiologyEducation.Repository
{
    public class AccessBookRepository
    {
        private readonly DataContext _context;

        public AccessBookRepository(DataContext context)
        {
            _context = context;
        }

        // Lấy danh sách toàn bộ access logs
        public List<AccessBook> GetAll()
        {
            return _context.AccessBooks.ToList();
        }

        // Lấy log theo User và Book
        public AccessBook? GetByUserAndBook(Guid userId, Guid bookId)
        {
            return _context.AccessBooks
                .FirstOrDefault(a => a.User_Id == userId && a.Book_Id == bookId);
        }

        // Ghi nhận lượt truy cập
        public bool RecordAccess(Guid userId, Guid bookId)
        {
            var existing = GetByUserAndBook(userId, bookId);
            if (existing != null)
            {
                existing.VisitedNumber += 1;
                existing.AccessedAt = DateTime.UtcNow;
                _context.AccessBooks.Update(existing);
            }
            else
            {
                var newAccess = new AccessBook
                {
                    AccessBook_Id = Guid.NewGuid(),
                    User_Id = userId,
                    Book_Id = bookId,
                    VisitedNumber = 1,
                    AccessedAt = DateTime.UtcNow
                };
                _context.AccessBooks.Add(newAccess);
            }

            return _context.SaveChanges() > 0;
        }
        public int TotalVisitedNumber(Guid bookId)
        {
            return _context.AccessBooks
                .Where(ab => ab.Book_Id == bookId)
                .Sum(ab => ab.VisitedNumber);
        }

        // Đếm tổng số user đã truy cập một sách (distinct user count)
        public int CountUniqueUsersForBook(Guid bookId)
        {
            return _context.AccessBooks.Count(a => a.Book_Id == bookId);
        }
    }
}
