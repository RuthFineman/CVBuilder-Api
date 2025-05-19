using CVBuilder.Core.Models;
using CVBuilder.Core.Repositories;
using Microsoft.EntityFrameworkCore;
namespace CVBuilder.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CVBuilderDbContext _context;

        public UserRepository(CVBuilderDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        //האם המשתמש חסום או לא
        public bool IsUserBlocked(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            return user != null && user.IsBlocked;
        }

    }
}

