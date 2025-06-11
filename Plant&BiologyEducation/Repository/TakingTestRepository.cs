using Plant_BiologyEducation.Data;
using Plant_BiologyEducation.Entity.Model;
using Microsoft.EntityFrameworkCore;
using Plant_BiologyEducation.Entity.DTO;

namespace Plant_BiologyEducation.Repository
{
    public class TakingTestRepository
    {
        private readonly DataContext _context;
        public TakingTestRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddTakingTest(TakingTest takingTest)
        {
            try
            {
                _context.TakingTests.Add(takingTest);
                return await Save();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<TakingTest>> GetTakingsByUser(Guid userId)
        {
            return await _context.TakingTests
                .Include(t => t.Test)
                .Include(t => t.User)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TakingDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TakingTestDTO>> GetTakingsByTest(string testId)
        {
            return await _context.TakingTests
                .Include(t => t.User)
                .Include(t => t.Test)
                .Where(t => t.TestId == testId)
                .Select(t => new TakingTestDTO
                {
                    UserId = t.UserId,
                    FullName = t.User.FullName,
                    TestId = t.TestId,
                    Result = t.Result,
                    TakingDate = t.TakingDate
                })
                .OrderByDescending(t => t.TakingDate)
                .ToListAsync();
        }
    }
}