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
            return _context.Users.Any(u => u.User_Id == id);
        }

        public ICollection<User> GetAllUsers()
        {
            // Include TakingTests and Test details for each user
            return _context.Users
                .OrderBy(u => u.Role)
                .ToList();
        }

        public ICollection<User> SearchUsersByFullName(string fullName)
        {
            return _context.Users
                .Where(u => u.FullName.Contains(fullName))
                .OrderBy(u => u.Role)
                .ToList();
        }

        public User GetUserById(Guid id)
        {
            return _context.Users
                .FirstOrDefault(u => u.User_Id == id);
        }

        public User GetUserByEmail(String email)
        {
            return _context.Users
                .FirstOrDefault(u => u.Email == email);
        }
        public User GetUserByAccount(string account)
        {
            return _context.Users.FirstOrDefault(u => u.Account == account);
        }

        public ICollection<User> GetInactiveUsers()
        {
            return _context.Users
                .Where(u => !u.IsActive)
                .ToList();
        }


        public bool CreateUser(User user)
        {
            _context.Users.Add(user);
            return Save();
        }
        public bool AccountExists(string account)
        {
            return _context.Users.Any(u => u.Account == account);
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
