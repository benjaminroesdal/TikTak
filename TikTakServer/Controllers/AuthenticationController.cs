using Microsoft.AspNetCore.Mvc;
using TikTakServer.ApplicationServices;

namespace TikTakServer.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly GoogleAuthService _authService;

        public AuthenticationController(GoogleAuthService googleAuthService)
        {
            _authService = googleAuthService;
        }

        [HttpPost("VerifyToken")]
        public async Task<IActionResult> VerifyToken([FromBody] string token)
        {
            var result = await _authService.VerifyTokenAsync(token);
            return Ok();
        }
    }
}
