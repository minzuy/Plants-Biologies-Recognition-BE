using Plant_BiologyEducation.Data;
using Plant_BiologyEducation.Entity.Model;

namespace Plant_BiologyEducation.Repository
{
    public class Plant_Biology_Animal_Repository
    {
        private readonly DataContext _context;

        public Plant_Biology_Animal_Repository(DataContext context)
        {
            _context = context;
        }
        public bool PBAExists(Guid id)
        {
            return _context.Plant_Biology_Animals.Any(u => u.Id == id);
        }

        public ICollection<Plant_Biology_Animals> GetAllEntity()
        {
            // Include TakingTests and Test details for each user
            return _context.Plant_Biology_Animals
                .ToList();
        }

        public ICollection<Plant_Biology_Animals> SearchByCommonName(string commonName)
        {
            return _context.Plant_Biology_Animals
                .Where(pba => pba.CommonName.Contains(commonName))
                .OrderBy(pba => pba.AverageLifeSpan)
                .ToList();
        }


        public Plant_Biology_Animals GetById(Guid id)
        {
            return _context.Plant_Biology_Animals
                .FirstOrDefault(pba => pba.Id == id);
        }

        public bool CreatePBA(Plant_Biology_Animals pba)
        {
            _context.Plant_Biology_Animals.Add(pba);
            return Save();
        }

        public bool UpdatePBA(Plant_Biology_Animals pba)
        {
            _context.Plant_Biology_Animals.Update(pba);
            return Save();
        }

        public bool DeletePBA(Plant_Biology_Animals pba)
        {
            _context.Plant_Biology_Animals.Remove(pba);
            return Save();
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}

