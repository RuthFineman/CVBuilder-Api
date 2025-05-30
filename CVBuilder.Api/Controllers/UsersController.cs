using CVBuilder.Api.Models;
using CVBuilder.Core.Services;
using CVBuilder.Core.Validators;
using CVBuilder.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

                return Ok(new { token, user.Id });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel userDto)
        {
            var user = await _userService.LoginAsync(userDto.Email, userDto.Password);

            if (user == null)
                return Unauthorized("Invalid credentials.");
            if (user.IsBlocked)
                return StatusCode(StatusCodes.Status403Forbidden, "משתמש חסום");

            var token = _authService.GenerateJwtToken(user.Email, user.Id, user.Role);
            Console.WriteLine(user.Id);
            return Ok(new { token, user.Id });
        }
        [HttpGet]
        [Authorize(Roles = "admin")] // 👈 רק למנהלים
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        [HttpPut("{id}/block/{isBlocked}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> BlockUser(int id, bool isBlocked)
        {
            var success = await _userService.SetBlockStatusAsync(id, isBlocked);

            if (!success)
                return NotFound(new { message = "User not found" });

            return NoContent();
        }
        [HttpGet("is-blocked/{userId}")]
        public IActionResult IsUserBlocked(int userId)
        {
            var isBlocked = _userService.IsUserBlocked(userId);
            return Ok(isBlocked);
        }
    }
}



