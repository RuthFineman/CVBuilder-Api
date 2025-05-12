using CVBuilder.Core.Models;
using CVBuilder.Core.Repositories;
using CVBuilder.Core.Services;
using CVBuilder.Core.Validators;
using System.Security.Cryptography;
using System.Text;

namespace CVBuilder.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public bool ValidateEmail(string email)
        {
            return UserValidator.IsValidEmail(email);
        }
        public async Task<bool> RegisterAsync(string fullName, string email, string password)
        {
            if (!ValidateEmail(email))
            {
                throw new InvalidOperationException("כתובת האימייל לא תקינה.");
            }
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                return false;

            var hashedPassword = HashPassword(password);

            var user = new User
            {
                FullName = fullName,
                Email = email,
                Password = hashedPassword,
                Role = "user",
                CVFiles = new List<FileCV>()
            };

            await _userRepository.AddAsync(user);
            return true;
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !VerifyPassword(password, user.Password))
                return null;

            return user;
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
        private bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            return HashPassword(inputPassword) == storedHashedPassword;
        }
    }
}