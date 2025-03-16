using CVBuilder.Api.Models;
using CVBuilder.Core.Models;
using CVBuilder.Core.Services;
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


        public UsersController(IUserService userService, AuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterModel userDto)
        {
            var result = await _userService.RegisterAsync(userDto.FullName, userDto.Email, userDto.Password, userDto.Role);
            if (!result)
                return BadRequest("User already exists.");

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel userDto)
        {
            var user = await _userService.LoginAsync(userDto.Email, userDto.Password);

            if (user == null)
                return Unauthorized("Invalid credentials.");

            var token = _authService.GenerateJwtToken(user.Email, user.Id,user.Role);  // יצירת ה-JWT

            return Ok(new { token });  // מחזיר את ה-Token למשתמש
        }
    }
}



