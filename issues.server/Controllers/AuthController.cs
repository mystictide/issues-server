using Microsoft.AspNetCore.Mvc;
using issues.server.Infrastructure.Helpers;
using issues.server.Infrastructure.Models.Main;
using issues.server.Infrastructure.Models.Response;
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
                var userData = new CompanyResponse();
                userData.ID = data.ID;
                userData.Name = data.Name;
                userData.Email = data.Email;
                userData.Token = data.Token;
                return Ok(userData);
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
                var userData = new CompanyResponse();
                userData.ID = data.ID;
                userData.Name = data.Name;
                userData.Email = data.Email;
                userData.Token = data.Token;
                return Ok(userData);
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
                var userData = new UserResponse();
                userData.ID = data.ID;
                userData.FirstName = data.FirstName;
                userData.LastName = data.LastName;
                userData.Email = data.Email;
                userData.Company = data.Company;
                userData.Role = data.Role;
                userData.Token = data.Token;
                return Ok(userData);
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpPost]
        [Route("cmail")]
        public async Task<IActionResult> CheckExistingEmail([FromBody] Dictionary<string, bool> data)
        {
            try
            {
                bool exists;
                string Email = data["email"].ToString();
                bool Company = data["company"];
                var UserID = AuthHelpers.CurrentUserID(HttpContext);
                if (UserID < 1)
                {
                    exists = await new AuthManager().CheckEmail(Company, Email, null);
                }
                else
                {
                    exists = await new AuthManager().CheckEmail(Company, Email, UserID);
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
