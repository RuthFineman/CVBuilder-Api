using CVBuilder.Core.Repositories;
using System.Text.RegularExpressions;

namespace CVBuilder.Core.Validators
{
    public class UserValidator
    {
        private readonly IUserRepository _userRepository;
        public UserValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public static bool IsValidEmail(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }
        public async Task<bool> IsValidPasswordAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null)
            {
                return false; 
            }
            if (password.Length < 5)
            {
                throw new InvalidOperationException("הסיסמה צריכה להיות לפחות 5 תווים.");
            }
            var hasNumber = new Regex(@"[0-9]+");
            var hasLetter = new Regex(@"[a-zA-Z]+");

            if (!hasNumber.IsMatch(password) || !hasLetter.IsMatch(password))
            {
                throw new InvalidOperationException("הסיסמה חייבת לכלול גם תו אלפביתי וגם תו ספרתי.");
            }
            return true;
        }
    }
}
