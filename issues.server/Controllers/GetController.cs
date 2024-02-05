﻿using Microsoft.AspNetCore.Mvc;
using issues.server.Infrastructure.Helpers;
using issues.server.Infrastructure.Data.Managers.Main;

namespace issues.server.Controllers
{
    [ApiController]
    [Route("get")]
    public class GetController : ControllerBase
    {
        private static int[] AuthorizedRoles = [1];

        [HttpGet]
        [Route("role")]
        public async Task<IActionResult> GetRole([FromQuery] int ID)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedRoles))
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
                if (AuthHelpers.Authorize(HttpContext, AuthorizedRoles))
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
        [Route("user")]
        public async Task<IActionResult> GetUser([FromQuery] int ID)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedRoles))
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
    }
}