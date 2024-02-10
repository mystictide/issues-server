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
                    filter.CompanyID = AuthHelpers.CurrentUserID(HttpContext);
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

        [HttpPost]
        [Route("users")]
        public async Task<IActionResult> FilterUsers([FromBody] Filter filter)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedRoles))
                {
                    filter.CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    var result = await new UserManager().FilteredList(filter);
                    return Ok(result);
                }
                return StatusCode(401, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }        

        [HttpPost]
        [Route("projects")]
        public async Task<IActionResult> FilterProjects([FromBody] Filter filter)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedRoles))
                {
                    filter.CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    var result = await new ProjectsManager().FilteredList(filter);
                    return Ok(result);
                }
                return StatusCode(401, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }    

        [HttpPost]
        [Route("issues")]
        public async Task<IActionResult> FilterIssues([FromBody] Filter filter)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedRoles))
                {
                    filter.CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    var result = await new IssuesManager().FilteredList(filter);
                    return Ok(result);
                }
                return StatusCode(401, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpPost]
        [Route("comments")]
        public async Task<IActionResult> FilterComments([FromBody] Filter filter)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedRoles))
                {
                    filter.CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    var result = await new IssuesManager().FilteredComments(filter);
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
