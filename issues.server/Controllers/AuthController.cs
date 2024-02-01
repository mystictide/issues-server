using Microsoft.AspNetCore.Mvc;
using issues.server.Infrastructure.Helpers;
using issues.server.Infrastructure.Models.Main;
using issues.server.Infrastructure.Data.Managers.Auth;

namespace issues.server.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Companies company)
        {
            try
            {
                var data = await new AuthManager().Register(company);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpPost]
        [Route("alogin")]
        public async Task<IActionResult> CompanyLogin([FromBody] Companies company)
        {
            try
            {
                var data = await new AuthManager().CompanyLogin(company);
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
                var data = await new AuthManager().Login(user);
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
                    exists = await new AuthManager().CheckEmail(email, null);
                }
                else
                {
                    exists = await new AuthManager().CheckEmail(email, userID);
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
