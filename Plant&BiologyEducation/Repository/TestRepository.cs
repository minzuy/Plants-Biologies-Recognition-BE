using Microsoft.EntityFrameworkCore;
using Plant_BiologyEducation.Data;
using Plant_BiologyEducation.Entity.Model;

namespace Plant_BiologyEducation.Repository
{
    public class TestRepository
    {
        private readonly DataContext _context;

        public TestRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<Test> GetAllTests()
        {
            return _context.Tests
                .Include(t => t.Questions)
                .Include(t => t.Creator)
                .ToList();
        }

        // Search chỉ theo ID với Contains
        public ICollection<Test> SearchTestsByIdContains(string partialId)
        {
            return _context.Tests
                .Include(t => t.Questions)
                .Include(t => t.Creator)
                .Where(t => t.Id.Contains(partialId))
                .OrderBy(t => t.Id)
                .ToList();
        }

        public ICollection<Test> GetTestsByCreatorName(String creatorName)
        {
            return _context.Tests
                            .Include(t => t.Questions)
                            .Include(t => t.Creator)
                            .Where(t => t.Creator != null &&
                                       (t.Creator.FullName.Contains(creatorName) ||
                                        t.Creator.Account.Contains(creatorName)))
                            .OrderByDescending(t => t.DateCreated)
                            .ToList();
        }


        public Test? GetTestById(string id)
        {
            return _context.Tests
                .Include(t => t.Questions)
                .Include(t => t.Creator)
                .FirstOrDefault(t => t.Id == id);
        }

        public bool CreateTest(Test test)
        {
            _context.Tests.Add(test);
            return Save();
        }

        public bool UpdateTest(Test test)
        {
            _context.Tests.Update(test);
            return Save();
        }

        public bool DeleteTest(Test test)
        {
            _context.Tests.Remove(test);
            return Save();
        }

        public bool TestExists(string id)
        {
            return _context.Tests.Any(t => t.Id == id);
        }

        private bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
