using Plant_BiologyEducation.Data;
using PlantBiologyEducation.Entity.Model;

public class AccessBiologyRepository
{
    private readonly DataContext _context;

    public AccessBiologyRepository(DataContext context)
    {
        _context = context;
    }
    public bool TrackAccess(Guid userId, Guid biologyId)
    {
        var existing = _context.AccessBiologies
            .FirstOrDefault(x => x.User_Id == userId && x.Oject_Id == biologyId);

        if (existing == null)
        {
            var newAccess = new AccessBiology
            {
                AccessBiology_Id = Guid.NewGuid(),
                User_Id = userId,
                Oject_Id = biologyId,
                VisitedNumber = 1,
                AccessedAt = DateTime.UtcNow
            };
            _context.AccessBiologies.Add(newAccess);
        }
        else
        {
            existing.VisitedNumber += 1;
            existing.AccessedAt = DateTime.UtcNow;
            _context.AccessBiologies.Update(existing);
        }

        return _context.SaveChanges() > 0;
    }

    public int CountUniqueUsersForBiology(Guid biologyId)
    {
        return _context.AccessBiologies
            .Where(x => x.Oject_Id == biologyId)
            .Select(x => x.User_Id)
            .Distinct()
            .Count();
    }

    public int TotalVisitedNumber(Guid biologyId)
    {
        return _context.AccessBiologies
            .Where(x => x.Oject_Id == biologyId)
            .Sum(x => x.VisitedNumber);
    }
}
