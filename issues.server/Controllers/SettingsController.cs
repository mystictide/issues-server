using Microsoft.AspNetCore.Mvc;
using issues.server.Infrastructure.Helpers;
using issues.server.Infrastructure.Data.Managers.Main;

namespace issues.server.Controllers
{
    [ApiController]
    [Route("settings")]
    public class SettingsController : ControllerBase
    {
        private static int AuthorizedAuthType = 1;

        [HttpPost]
        [Route("update/password")]
        public async Task<IActionResult> ChangePassword([FromBody] Dictionary<string, string> data)
        {
            try
            {
                string currentPassword = data["currentPassword"];
                string newPassword = data["newPassword"];
                if (AuthHelpers.Authorize(HttpContext, AuthorizedAuthType))
                {
                    var result = await new UserManager().ChangePassword(AuthHelpers.CurrentUserID(HttpContext), currentPassword, newPassword);
                    return Ok(result);
                }
                else
                {
                    return StatusCode(401, "Authorization failed");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpPost]
        [Route("update/email")]
        public async Task<IActionResult> UpdateEmail([FromBody] string email)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedAuthType))
                {
                    var result = await new UserManager().UpdateEmail(AuthHelpers.CurrentUserID(HttpContext), email);
                    if (result == null)
                    {
                        return StatusCode(401, "Email already in use");
                    }
                    return Ok(result);
                }
                else
                {
                    return StatusCode(401, "Authorization failed");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }
    }
}
