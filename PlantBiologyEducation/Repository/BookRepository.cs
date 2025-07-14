using Microsoft.EntityFrameworkCore;
using Plant_BiologyEducation.Data;
using Plant_BiologyEducation.Entity.Model;

namespace Plant_BiologyEducation.Repository
{
    public class BookRepository
    {
        private readonly DataContext _context;

        public BookRepository(DataContext context)
        {
            _context = context;
        }

        // Lấy tất cả sách
        public ICollection<Book> GetAllBooks()
        {
            return _context.Books
                .AsSplitQuery()
                .Include(b => b.Chapters)
                    .ThenInclude(c => c.Lessons)
                    .ThenInclude(l => l.RelatedSpecies)
                .OrderBy(b => b.Book_Title)
                .ToList();
        }

        public Book GetBookById(Guid id)
        {
            return _context.Books
                .AsSplitQuery()
                .Include(b => b.Chapters)
                    .ThenInclude(c => c.Lessons)
                    .ThenInclude(l => l.RelatedSpecies)
                .FirstOrDefault(b => b.Book_Id == id);
        }
        // Tìm sách theo tiêu đề
        public ICollection<Book> SearchBooksByTitle(string title)
        {
            return _context.Books
                .Where(b => b.Book_Title.ToLower().Contains(title.ToLower()))
                .Include(b => b.Chapters)
                    .ThenInclude(c => c.Lessons)
                    .ThenInclude(l => l.RelatedSpecies) // Thêm dòng này
                .OrderBy(b => b.Book_Title)
                .ToList();
        }
        public ICollection<Book> GetPendingBooks()
        {
            return _context.Books
                .Where(b => b.Status == "Pending")
                .AsSplitQuery()
                .Include(b => b.Chapters)
                    .ThenInclude(c => c.Lessons)
                    .ThenInclude(l => l.RelatedSpecies)
                .OrderBy(b => b.Book_Title)
                .ToList();
        }
        // Thêm sách mới
        public bool CreateBook(Book book)
        {
            _context.Books.Add(book);
            return Save();
        }

        // Cập nhật sách
        public bool UpdateBook(Book book)
        {
            _context.Books.Update(book);
            return Save();
        }

        // Xoá sách
        public bool DeleteBook(Book book)
        {
            _context.Books.Remove(book);
            return Save();
        }

        // Kiểm tra sách có tồn tại không
        public bool BookExists(Guid id)
        {
            return _context.Books.Any(b => b.Book_Id == id);
        }

        // Lưu thay đổi
        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }

        internal object GetAllBooksForStudents()
        {
            return _context.Books
                 .Where(b => b.Status == "Approved")
                  .AsSplitQuery()
                  .Include(b => b.Chapters)
                  .ThenInclude(c => c.Lessons)
                  .ThenInclude(l => l.RelatedSpecies)
                  .OrderBy(b => b.Book_Title)
                  .ToList();
        }
    }
}