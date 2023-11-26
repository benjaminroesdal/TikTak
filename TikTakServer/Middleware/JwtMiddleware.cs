using TikTakServer.Models;

namespace TikTakServer.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, UserRequestAndClaims requestClaims)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                requestClaims.UserId = "1"/*context.User.FindFirst("user_id")?.Value*/;
                requestClaims.ProfileImage = "PhotoUrl"/*context.User.FindFirst("profile_img")?.Value*/;
                requestClaims.Email = "mbvest50@gmail.com"/*context.User.FindFirst("user_email")?.Value*/;
            }

            await _next(context);
        }
    }
}
