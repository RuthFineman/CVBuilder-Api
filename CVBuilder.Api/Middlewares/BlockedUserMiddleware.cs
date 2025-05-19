using CVBuilder.Core.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace CVBuilder.Api.Middlewares
{
    public class BlockedUserMiddleware
    {
        private readonly RequestDelegate _next;

        public BlockedUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
        {
            var path = context.Request.Path.ToString().ToLower();

                if (path.StartsWith("/file-cv") && (context.Request.Method == "POST" || context.Request.Method == "PUT" || context.Request.Method == "DELETE"))
                {
                string userId = context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
 

                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await userRepository.GetByIdAsync(int.Parse(userId));
                    if (user != null && user.IsBlocked)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync("המשתמש חסום ואינו יכול לבצע פעולה זו.");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
