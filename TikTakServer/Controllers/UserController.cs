using Microsoft.AspNetCore.Mvc;
using TikTakServer.Models.Business;

namespace TikTakServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly DatabaseSeeder _seeder;

        public UserController(ILogger<UserController> logger, DatabaseSeeder seeder)
        {
            _logger = logger;
            _seeder = seeder;
        }

        [HttpPost]
        [Route("SeedDb")]
        public async Task<IActionResult> SeedDb()
        {
            _seeder.SeedDb();
            return Ok();
        }

    }
}
