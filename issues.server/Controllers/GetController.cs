using Microsoft.AspNetCore.Mvc;
using issues.server.Infrastructure.Helpers;
using issues.server.Infrastructure.Data.Managers.Main;

namespace issues.server.Controllers
{
    [ApiController]
    [Route("get")]
    public class GetController : ControllerBase
    {
        [HttpGet]
        [Route("role")]
        public async Task<IActionResult> GetRole([FromQuery] int ID)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1]))
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

        [HttpGet]
        [Route("roles")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1, 2]))
                {
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    var role = await new RolesManager().GetCompanyRoles(CompanyID);
                    return Ok(role);
                }
                return StatusCode(401, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpGet]
        [Route("project")]
        public async Task<IActionResult> GetProject([FromQuery] int ID)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1, 3]))
                {
                    var project = await new ProjectsManager().Get(ID);
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    if (CompanyID == project?.CompanyID)
                    {
                        return Ok(project);
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

        [HttpGet]
        [Route("projects")]
        public async Task<IActionResult> GetProjects([FromQuery] int? limit)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1, 2, 3, 4, 5]))
                {
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    var projects = await new ProjectsManager().GetCompanyProjects(CompanyID, limit);
                    return Ok(projects);
                }
                return StatusCode(401, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpGet]
        [Route("issue")]
        public async Task<IActionResult> GetIssue([FromQuery] int ID)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1, 3, 4]))
                {
                    var issue = await new IssuesManager().Get(ID);
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    if (CompanyID == issue?.Project?.CompanyID)
                    {
                        return Ok(issue);
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

        [HttpGet]
        [Route("issues")]
        public async Task<IActionResult> GetIssues([FromQuery] int? limit)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1, 2, 3, 4, 5]))
                {
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    var issues = await new IssuesManager().GetCompanyIssues(CompanyID, limit);
                    return Ok(issues);
                }
                return StatusCode(401, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpGet]
        [Route("comment")]
        public async Task<IActionResult> GetComment([FromQuery] int ID, [FromQuery] int IssueID)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1, 2, 3, 4, 5]))
                {
                    var issue = await new IssuesManager().Get(IssueID);
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    if (CompanyID == issue?.Project?.CompanyID)
                    {
                        var comment = await new IssuesManager().GetComment(ID);
                        return Ok(comment);
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

        [HttpGet]
        [Route("comments")]
        public async Task<IActionResult> GetComments([FromQuery] int? limit)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1, 2, 3, 4, 5]))
                {
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    var comments = await new IssuesManager().GetCompanyComments(CompanyID, limit);
                    return Ok(comments);
                }
                return StatusCode(401, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpGet]
        [Route("user")]
        public async Task<IActionResult> GetUser([FromQuery] int ID)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1, 2]))
                {
                    var user = await new UserManager().Get(ID);
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    if (CompanyID == user?.Company?.ID)
                    {
                        return Ok(user);
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

        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1, 2, 3, 4, 5]))
                {
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    var users = await new UserManager().GetCompanyUsers(CompanyID);
                    return Ok(users);
                }
                return StatusCode(401, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpGet]
        [Route("stats/issues")]
        public async Task<IActionResult> GetIssueStats()
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1, 2, 3, 4, 5]))
                {
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    var users = await new IssuesManager().GetStatistics(CompanyID);
                    return Ok(users);
                }
                return StatusCode(401, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpGet]
        [Route("stats/projects")]
        public async Task<IActionResult> GetProjectStats()
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, [1, 2, 3, 4, 5]))
                {
                    var CompanyID = AuthHelpers.CurrentUserID(HttpContext);
                    var users = await new ProjectsManager().GetStatistics(CompanyID);
                    return Ok(users);
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
