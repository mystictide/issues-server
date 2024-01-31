using Microsoft.AspNetCore.Mvc;
using issues.server.Infrastructure.Helpers;
using issues.server.Infrastructure.Managers.Users;

namespace issues.server.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
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

        [HttpPost]
        [Route("update/bio")]
        public async Task<IActionResult> UpdateBio([FromBody] string bio)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedAuthType))
                {
                    var result = await new UserManager().UpdateBio(AuthHelpers.CurrentUserID(HttpContext), bio);
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
        [Route("follow")]
        public async Task<IActionResult> ManageFollow([FromBody] int TargetID)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedAuthType))
                {
                    var res = await new UserManager().ManageFollow(TargetID, AuthHelpers.CurrentUserID(HttpContext));
                    return Ok(res);
                }
                return StatusCode(401, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpPost]
        [Route("block")]
        public async Task<IActionResult> ManageBlock([FromBody] int TargetID)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedAuthType))
                {
                    var res = await new UserManager().ManageBlock(TargetID, AuthHelpers.CurrentUserID(HttpContext));
                    return Ok(res);
                }
                return StatusCode(401, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }
    }
}
