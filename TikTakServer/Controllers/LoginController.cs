using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TikTakServer.Models.Business;

namespace TikTakServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : Controller
    {
        //This method is not done, return is just there for the method to not fail when building.
        [HttpGet]
        public Task<IActionResult> Login([FromBody] User user)
        {
            return Task.FromResult<IActionResult>(Ok());
        }
    }
}
