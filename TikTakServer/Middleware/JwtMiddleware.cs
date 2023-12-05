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

        /// <summary>
        /// Looks for claims from JWT in the HttpContext and populates the UserRequestAndClaims object with the data.
        /// </summary>
        /// <param name="requestClaims">Object to populate with data from request.</param>
        public async Task Invoke(HttpContext context, UserRequestAndClaims requestClaims)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                requestClaims.ProfileImage = context.User.FindFirst("profile_img")?.Value;
                requestClaims.Email = context.User.FindFirst("user_email")?.Value;
            }

            await _next(context);
        }
    }
}
