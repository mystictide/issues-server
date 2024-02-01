using Microsoft.AspNetCore.Mvc;
using issues.server.Infrastructure.Helpers;
using issues.server.Infrastructure.Models.Main;
using issues.server.Infrastructure.Managers.Main;

namespace issues.server.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Users user)
        {
            try
            {
                var data = await new UserManager().Register(user);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] Users user)
        {
            try
            {
                var data = await new UserManager().Login(user);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpPost]
        [Route("cmail")]
        public async Task<IActionResult> CheckExistingEmail([FromBody] string email)
        {
            try
            {
                bool exists;
                var userID = AuthHelpers.CurrentUserID(HttpContext);
                if (userID < 1)
                {
                    exists = await new UserManager().CheckEmail(email, null);
                }
                else
                {
                    exists = await new UserManager().CheckEmail(email, userID);
                }
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }
    }
}
