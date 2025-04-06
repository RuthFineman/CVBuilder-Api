using CVBuilder.Api.Models;
using CVBuilder.Core.Models;
using CVBuilder.Core.Services;
using CVBuilder.Core.Validators;
using CVBuilder.Data;
using CVBuilder.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CVBuilder.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AuthService _authService;
        private readonly UserValidator _userValidator;
        public UsersController(IUserService userService, AuthService authService, UserValidator userValidator)
        {
            _userService = userService;
            _authService = authService;
            _userValidator = userValidator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterModel userDto)
        {
            Console.WriteLine($"FullName: {userDto.FullName}, Email: {userDto.Email}, Password: {userDto.Password}");

            try
            {
               

                var isValidPassword = await _userValidator.IsValidPasswordAsync(userDto.Email, userDto.Password);
                if (!isValidPassword)
                {
                    return BadRequest("הסיסמה לא תקינה.");
                }
                var result = await _userService.RegisterAsync(userDto.FullName, userDto.Email, userDto.Password);
                if (!result)
                    return BadRequest("User already exists.");
                var user = await _userService.LoginAsync(userDto.Email, userDto.Password);
                var token = _authService.GenerateJwtToken(user.Email, user.Id, user.Role);

                // החזרת ה-Token למשתמש
                return Ok(new { token, user.Id });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // החזרת הודעת שגיאה אם המייל לא תקין
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message); // טיפול בשגיאות כלליות
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel userDto)
        {
            var user = await _userService.LoginAsync(userDto.Email, userDto.Password);

            if (user == null)
                return Unauthorized("Invalid credentials.");

            var token = _authService.GenerateJwtToken(user.Email, user.Id,user.Role);  // יצירת ה-JWT
            Console.WriteLine(user.Id);
            return Ok(new { token,user.Id });  // מחזיר את ה-Token למשתמש
        }
    }
}



