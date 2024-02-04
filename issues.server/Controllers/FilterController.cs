using Microsoft.AspNetCore.Mvc;
using issues.server.Infrastructure.Helpers;
using issues.server.Infrasructure.Models.Helpers;
using issues.server.Infrastructure.Data.Managers.Main;

namespace issues.server.Controllers
{
    [ApiController]
    [Route("filter")]
    public class FilterController : ControllerBase
    {
        private static int[] AuthorizedRoles = [1];

        [HttpPost]
        [Route("roles")]
        public async Task<IActionResult> FilterRoles([FromBody] Filter filter)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedRoles))
                {
                    var result = await new RolesManager().FilteredList(filter);
                    return Ok(result);
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
