using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Service
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateJwtToken(string email, int userId, string role)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, role)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),  // הגדרת הזמן שבו ה-Token יפוג
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        //    public string GenerateJwtToken(string email, int userId, string role)
        //    {
        //        var claims = new[]
        //    {
        //    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        //    new Claim(ClaimTypes.Email, email),
        //    new Claim(ClaimTypes.Role, role) 
        //};

        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //        var token = new JwtSecurityToken(
        //            issuer: _configuration["Jwt:Issuer"],
        //            audience: _configuration["Jwt:Audience"],
        //            claims: claims,
        //            expires: DateTime.Now.AddHours(1),  // הגדרת הזמן שבו ה-Token יפוג
        //            signingCredentials: credentials
        //        );

        //        return new JwtSecurityTokenHandler().WriteToken(token);
        //    }
        //public int GetUserIdFromToken(string token)
        //{
        //    if (string.IsNullOrEmpty(token))
        //    {
        //        throw new ArgumentException("Token is null or empty.");
        //    }

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        //    if (jsonToken == null)
        //    {
        //        throw new ArgumentException("Invalid token.");
        //    }

        //    var userIdClaim = jsonToken?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        //    if (int.TryParse(userIdClaim, out var userId))
        //    {
        //        return userId;
        //    }

        //    throw new ArgumentException("UserId not found in token.");
        //}
    }
}

