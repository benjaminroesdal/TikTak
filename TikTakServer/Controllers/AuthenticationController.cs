using Microsoft.AspNetCore.Mvc;
using TikTakServer.ApplicationServices;
using TikTakServer.Models.Business;

namespace TikTakServer.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService _authService;

        public AuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] CreateUserRequest userRequest)
        {
            if(userRequest == null)
            {
                return BadRequest("User request was not specified");
            }
            if (String.IsNullOrEmpty(userRequest.GoogleAccessToken) || 
                String.IsNullOrEmpty(userRequest.ImageUrl) || 
                String.IsNullOrEmpty(userRequest.FulLName))
            {
                return BadRequest("One or more parameters was not specified in the request sent");
            }

            var result = await _authService.Login(userRequest.GoogleAccessToken, userRequest.FulLName, userRequest.ImageUrl);
            return Ok(result);
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] string token)
        {
            if(String.IsNullOrEmpty(token))
            {
                return BadRequest("Token for logout was incorrect or not specified");
            }

            await _authService.Logout(token);
            return Ok();
        }

        [HttpPost("RefreshAccessToken")]
        public async Task<IActionResult> RefreshAccessToken([FromBody]string refreshToken)
        {
            if (String.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("Access token for refreshing was incorrect or not specified");
            }

            var result = await _authService.RefreshAccessToken(refreshToken);
            return Ok(result);
        }
    }
}
