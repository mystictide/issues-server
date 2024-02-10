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

        [HttpPost]
        [Route("user")]
        public async Task<IActionResult> ManageUser([FromBody] Users entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1]))
                {
                    var user = await new UserManager().Get(entity.ID);
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    if (user != null)
                    {
                        if (CompanyID == user?.Company.ID)
                        {
                            var result = await new UserManager().Manage(entity);
                            return Ok(result);
                        }
                    }
                    else
                    {
                        if (CompanyID == entity.Company?.ID)
                        {
                            var result = await new UserManager().Manage(entity);
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

        [HttpPost]
        [Route("project")]
        public async Task<IActionResult> ManageProject([FromBody] Projects entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1]))
                {
                    var project = await new ProjectsManager().Get(entity.ID);
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    if (project != null)
                    {
                        if (CompanyID == project?.CompanyID)
                        {
                            var result = await new ProjectsManager().Manage(entity);
                            return Ok(result);
                        }
                    }
                    else
                    {
                        if (CompanyID == entity.CompanyID)
                        {
                            var result = await new ProjectsManager().Manage(entity);
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

        [HttpPost]
        [Route("issue")]
        public async Task<IActionResult> ManageIssue([FromBody] Issues entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1]))
                {
                    var issue = await new IssuesManager().Get(entity.ID);
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    if (issue != null)
                    {
                        if (CompanyID == issue?.Project?.CompanyID)
                        {
                            var result = await new IssuesManager().Manage(entity);
                            return Ok(result);
                        }
                    }
                    else
                    {
                        if (CompanyID == entity.Project?.CompanyID)
                        {
                            var result = await new IssuesManager().Manage(entity);
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

        [HttpPost]
        [Route("issue/type")]
        public async Task<IActionResult> ManageIssueType([FromBody] Issues entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1]))
                {
                    var issue = await new IssuesManager().Get(entity.ID);
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    if (issue != null)
                    {
                        if (CompanyID == issue?.Project?.CompanyID)
                        {
                            var result = await new IssuesManager().ManageType(issue.ID, (int)entity.Type);
                            return Ok(result);
                        }
                    }
                    else
                    {
                        if (CompanyID == entity?.Project?.CompanyID)
                        {
                            var result = await new IssuesManager().ManageType(issue.ID, (int)entity.Type);
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

        [HttpPost]
        [Route("issue/status")]
        public async Task<IActionResult> ManageIssueStatus([FromBody] Issues entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1]))
                {
                    var issue = await new IssuesManager().Get(entity.ID);
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    if (issue != null)
                    {
                        if (CompanyID == issue?.Project?.CompanyID)
                        {
                            var result = await new IssuesManager().ManageStatus(issue.ID, (int)entity.Status);
                            return Ok(result);
                        }
                    }
                    else
                    {
                        if (CompanyID == entity?.Project?.CompanyID)
                        {
                            var result = await new IssuesManager().ManageStatus(issue.ID, (int)entity.Status);
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

        [HttpPost]
        [Route("issue/priority")]
        public async Task<IActionResult> ManageIssuePriority([FromBody] Issues entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1]))
                {
                    var issue = await new IssuesManager().Get(entity.ID);
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    if (issue != null)
                    {
                        if (CompanyID == issue?.Project?.CompanyID)
                        {
                            var result = await new IssuesManager().ManagePriority(issue.ID, (int)entity.Priority);
                            return Ok(result);
                        }
                    }
                    else
                    {
                        if (CompanyID == entity?.Project?.CompanyID)
                        {
                            var result = await new IssuesManager().ManagePriority(issue.ID, (int)entity.Priority);
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

        [HttpPost]
        [Route("comment")]
        public async Task<IActionResult> ManageComment([FromBody] Comments entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1]))
                {
                    var issue = await new IssuesManager().Get(entity.IssueID);
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    if (issue != null)
                    {
                        if (CompanyID == issue?.Project?.CompanyID)
                        {
                            var result = await new IssuesManager().ManageComment(entity);
                            return Ok(result);
                        }
                    }
                    else
                    {
                        if (CompanyID == entity.User?.Company?.ID)
                        {
                            var result = await new IssuesManager().ManageComment(entity);
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
