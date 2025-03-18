using CVBuilder.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            // בדיקת אם הסיסמה לא קיימת במערכת
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null)
            {
                return false; 
            }

            // בדיקת אורך הסיסמה
            if (password.Length < 5)
            {
                throw new InvalidOperationException("הסיסמה צריכה להיות לפחות 5 תווים.");
            }

            // בדיקת אם הסיסמה כוללת גם תו ספרתי וגם תו אלפביתי
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
