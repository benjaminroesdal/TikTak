using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TikTakServer.ApplicationServices;

namespace TikTakServer.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly GoogleAuthService _googleAuthService;
        private readonly AuthenticationService _authService;

        public AuthenticationController(GoogleAuthService googleAuthService, AuthenticationService authService)
        {
            _googleAuthService = googleAuthService;
            _authService = authService;
        }

        [HttpPost("VerifyToken")]
        public async Task<IActionResult> VerifyToken([FromBody] string token)
        {
            var result = await _googleAuthService.VerifyTokenAsync(token);
            return Ok();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] CreateUserRequest userRequest)
        {
            var result = await _authService.Login(userRequest.GoogleAccessToken, userRequest.FulLName, userRequest.ImageUrl, userRequest.Longitude, userRequest.Latitude);
            return Ok(result);
        }

        [HttpPost("RefreshAccessToken")]
        public async Task<IActionResult> RefreshAccessToken([FromBody]string refreshToken)
        {
            var result = await _authService.RefreshAccessToken(refreshToken);
            return Ok(result);
        }
    }

    public class CreateUserRequest
    {
        public string GoogleAccessToken { get; set; }
        public string FulLName { get; set; }
        public string ImageUrl { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
