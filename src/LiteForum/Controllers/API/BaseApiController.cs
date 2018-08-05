
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using LiteForum.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace LiteForum.Controllers.API
{
    [Authorize(policy: AppConstants.String.Policies.Authenticated)]
    [ApiController]
    public class BaseApiController : Controller
    {
        protected string UserId => User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
        protected bool IsMember => User.IsInRole(AppConstants.String.Roles.Member);
        protected bool IsAdmin => User.IsInRole(AppConstants.String.Roles.Admin);
    }
}
