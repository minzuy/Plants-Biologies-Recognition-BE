using Microsoft.EntityFrameworkCore;
using Plant_BiologyEducation.Data;
using Plant_BiologyEducation.Entity.Model;

namespace Plant_BiologyEducation.Repository
{
    public class UserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public bool UserExists(Guid id)
        {
            return _context.Users.Any(u => u.Id == id);
        }

        public ICollection<User> GetAllUsers()
        {
            // Include TakingTests and Test details for each user
            return _context.Users
                .Include(u => u.Takings)
                    .ThenInclude(t => t.Test)
                .ToList();
        }

        public User GetUserById(Guid id)
        {
            return _context.Users
                .Include(u => u.Takings)
                    .ThenInclude(t => t.Test)
                .FirstOrDefault(u => u.Id == id);
        }

        public bool CreateUser(User user)
        {
            _context.Users.Add(user);
            return Save();
        }

        public bool UpdateUser(User user)
        {
            _context.Users.Update(user);
            return Save();
        }

        public bool DeleteUser (User user)
        {
            _context.Users.Remove(user);
            return Save();
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
