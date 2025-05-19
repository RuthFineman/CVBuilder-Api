namespace CVBuilder.Api.Middlewares
{
    public static class BlockedUserMiddlewareExtensions
    {
        public static IApplicationBuilder UseBlockedUserMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BlockedUserMiddleware>();
        }
    }
}
