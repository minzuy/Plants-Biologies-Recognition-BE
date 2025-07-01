using Plant_BiologyEducation.Data;
using PlantBiologyEducation.Entity.Model;

public class AccessLessonRepository
{
    private readonly DataContext _context;

    public AccessLessonRepository(DataContext context)
    {
        _context = context;
    }
    public bool TrackAccess(Guid userId, Guid lessonId)
    {
        var existing = _context.AccessLessons
            .FirstOrDefault(x => x.User_Id == userId && x.Lesson_Id == lessonId);

        if (existing == null)
        {
            var newAccess = new AccessLesson
            {
                AccessLesson_Id = Guid.NewGuid(),
                User_Id = userId,
                Lesson_Id = lessonId,
                VisitedNumber = 1,
                AccessedAt = DateTime.UtcNow
            };
            _context.AccessLessons.Add(newAccess);
        }
        else
        {
            existing.VisitedNumber += 1;
            existing.AccessedAt = DateTime.UtcNow;
            _context.AccessLessons.Update(existing);
        }

        return _context.SaveChanges() > 0;
    }

    public int CountUniqueUsersForLesson(Guid lessonId)
    {
        return _context.AccessLessons
            .Where(x => x.Lesson_Id == lessonId)
            .Select(x => x.User_Id)
            .Distinct()
            .Count();
    }

    public int TotalVisitedNumber(Guid lessonId)
    {
        return _context.AccessLessons
            .Where(x => x.Lesson_Id == lessonId)
            .Sum(x => x.VisitedNumber);
    }
}
