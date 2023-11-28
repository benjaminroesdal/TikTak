using TikTakServer.Models.Business;

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
            if (context.User.Identity.IsAuthenticated)
            {
                requestClaims.UserId = context.User.FindFirst("user_id")?.Value;
                requestClaims.ProfileImage = context.User.FindFirst("profile_img")?.Value;
                requestClaims.Email = context.User.FindFirst("user_email")?.Value;
                requestClaims.Country = context.User.FindFirst("user_country")?.Value;
            }

            await _next(context);
        }
    }
}
