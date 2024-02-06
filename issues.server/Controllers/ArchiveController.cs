using Microsoft.AspNetCore.Mvc;
using issues.server.Infrastructure.Helpers;
using issues.server.Infrastructure.Models.Main;
using issues.server.Infrastructure.Data.Managers.Main;

namespace issues.server.Controllers
{
    [ApiController]
    [Route("archive")]
    public class ArchiveController : ControllerBase
    {
        private static int[] AuthorizedRoles = [1];

        [HttpPost]
        [Route("role")]
        public async Task<IActionResult> ArchiveRole([FromBody] Roles entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedRoles))
                {
                    var role = await new RolesManager().Get(entity.ID);
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    if (CompanyID == role?.CompanyID)
                    {
                        var result = await new RolesManager().Archive(entity);
                        return Ok(result);
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

        [HttpPost]
        [Route("project")]
        public async Task<IActionResult> ArchiveProject([FromBody] Projects entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedRoles))
                {
                    var project = await new ProjectsManager().Get(entity.ID);
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    if (CompanyID == project?.CompanyID)
                    {
                        var result = await new ProjectsManager().Archive(entity);
                        return Ok(result);
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

        [HttpPost]
        [Route("issue")]
        public async Task<IActionResult> ArchiveIssue([FromBody] Issues entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedRoles))
                {
                    var issue = await new IssuesManager().Get(entity.ID);
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    if (CompanyID == issue?.Project?.CompanyID)
                    {
                        var result = await new IssuesManager().Archive(entity);
                        return Ok(result);
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
