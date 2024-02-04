using Microsoft.AspNetCore.Mvc;
using issues.server.Infrastructure.Helpers;
using issues.server.Infrastructure.Models.Main;
using issues.server.Infrastructure.Data.Managers.Main;

namespace issues.server.Controllers
{
    [ApiController]
    [Route("manage")]
    public class ManagementController : ControllerBase
    {
        private static int[] AuthorizedRoles = [1, 2];

        [HttpPost]
        [Route("role")]
        public async Task<IActionResult> ManageRole([FromBody] Roles entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1]))
                {
                    var role = await new RolesManager().Get(entity.ID);
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    if (role != null)
                    {
                        if (CompanyID == role?.CompanyID)
                        {
                            var result = await new RolesManager().Manage(entity);
                            return Ok(result);
                        }
                    }
                    else
                    {
                        if (CompanyID == entity.CompanyID)
                        {
                            var result = await new RolesManager().Manage(entity);
                            return Ok(result);
                        }
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
