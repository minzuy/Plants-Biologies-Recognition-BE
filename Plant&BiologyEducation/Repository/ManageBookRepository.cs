using Microsoft.EntityFrameworkCore;
using Plant_BiologyEducation.Data;
using Plant_BiologyEducation.Entity.Model;

namespace Plant_BiologyEducation.Repository
{
    public class ManageBookRepository
    {
        private readonly DataContext _context;

        public ManageBookRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<ManageBook>> GetAllAsync()
        {
            return await _context.ManageBooks
                .Include(mb => mb.User)
                .Include(mb => mb.Book)
                .ToListAsync();
        }

        public async Task<ManageBook?> GetByIdAsync(Guid userId, Guid bookId)
        {
            return await _context.ManageBooks
                .Include(mb => mb.User)
                .Include(mb => mb.Book)
                .FirstOrDefaultAsync(mb => mb.User_Id == userId && mb.Book_Id == bookId);
        }

        public async Task<List<ManageBook>> GetByUserIdAsync(Guid userId)
        {
            return await _context.ManageBooks
                .Include(mb => mb.Book)
                .Where(mb => mb.User_Id == userId)
                .ToListAsync();
        }

        public async Task<List<ManageBook>> GetByBookIdAsync(Guid bookId)
        {
            return await _context.ManageBooks
                .Include(mb => mb.User)
                .Where(mb => mb.Book_Id == bookId)
                .ToListAsync();
        }

        public async Task AddAsync(ManageBook manageBook)
        {
            await _context.ManageBooks.AddAsync(manageBook);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateDateAsync(Guid userId, Guid bookId)
        {
            var entry = await _context.ManageBooks
                .FirstOrDefaultAsync(mb => mb.User_Id == userId && mb.Book_Id == bookId);

            if (entry == null)
                return false;

            entry.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task TrackBookEditAsync(Guid userId, Guid bookId)
        {
            var existing = await GetByIdAsync(userId, bookId);
            if (existing == null)
            {
                await AddAsync(new ManageBook { User_Id = userId, Book_Id = bookId, UpdatedDate = DateTime.UtcNow });
            }
            else
            {
                existing.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid bookId)
        {
            return await _context.ManageBooks
                .AnyAsync(mb => mb.User_Id == userId && mb.Book_Id == bookId);
        }
    }
}