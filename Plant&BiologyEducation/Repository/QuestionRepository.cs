using Microsoft.EntityFrameworkCore;
using Plant_BiologyEducation.Data;
using Plant_BiologyEducation.Entity.Model;

namespace Plant_BiologyEducation.Repository
{

    public class QuestionRepository
    {
        private readonly DataContext _context;

        public QuestionRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<Question> GetAllQuestions()
        {
            return _context.Questions
                .Include(q => q.Test)
                .ToList();
        }

        public Question? GetQuestionById(int id)
        {
            return _context.Questions
                .Include(q => q.Test)
                .FirstOrDefault(q => q.Id == id);
        }

        public ICollection<Question> GetQuestionsByTestId(String testId)
        {
            return _context.Questions
                .Where(q => q.TestId == testId)
                .Include(q => q.Test)
                .ToList();
        }

        public bool CreateQuestion(Question question)
        {
            _context.Questions.Add(question);
            return Save();
        }

        public bool UpdateQuestion(Question question)
        {
            _context.Questions.Update(question);
            return Save();
        }

        public bool DeleteQuestion(Question question)
        {
            _context.Questions.Remove(question);
            return Save();
        }

        public bool QuestionExists(int id)
        {
            return _context.Questions.Any(q => q.Id == id);
        }

        private bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
