using Microsoft.AspNetCore.Mvc;
using issues.server.Infrastructure.Helpers;
using issues.server.Infrastructure.Data.Managers.Main;

namespace issues.server.Controllers
{
    [ApiController]
    [Route("get")]
    public class GetController : ControllerBase
    {
        private static int AuthorizedAuthType = 1;

        [HttpGet]
        [Route("role")]
        public async Task<IActionResult> GetRole([FromQuery] int ID)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedAuthType))
                {
                    var role = await new RolesManager().Get(ID);
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    if (CompanyID == role?.CompanyID)
                    {
                        return Ok(role);
                    }
                    return StatusCode(401, "Access denied");
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
