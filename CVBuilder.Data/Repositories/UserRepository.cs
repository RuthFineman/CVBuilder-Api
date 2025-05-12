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
    }
}

