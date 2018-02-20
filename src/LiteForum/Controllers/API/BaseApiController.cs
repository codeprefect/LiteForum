
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using LiteForum.Helpers;

namespace LiteForum.Controllers.API
{
    public class BaseApiController : Controller
    {
        protected string UserId => User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
        protected bool IsMember => User.IsInRole("Member");
        protected bool IsAdmin => User.IsInRole(AppConstants.Roles.Admin);
    }
}
