using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using LiteForum.Models;
using LiteForum.ViewModels;
using LiteForum.Helpers;

namespace LiteForum.Controllers
{
    [Route("/api/[action]")]
    public class AuthController : Controller
    {
        private readonly SignInManager<LiteForumUser> _signInManager;
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<LiteForumUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public AuthController(SignInManager<LiteForumUser> signInManager, ILogger<AuthController> logger, UserManager<LiteForumUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVModel model)
        {
            if (!ModelState.IsValid) return BadRequest("request object is not a valid");
            {
                var user = new LiteForumUser { UserName = model.Username, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, AppConstants.Roles.Member); // add user to default member role

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    return Ok(new {
                        username = user.UserName,
                        email = user.Email,
                        status = "successfull"
                    });
                }
                AddErrors(result);
            }

            return Ok(ModelState);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginVModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);

                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                    if (result.Succeeded)
                    {
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Secret"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                            _config["Tokens:Audience"],
                            await GetValidClaims(user),
                            expires: DateTime.Now.AddDays(7),
                            signingCredentials: creds);

                        _logger.LogInformation($"Created token for {user.UserName}");

                        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), expiration = token.ValidTo });
                    }
                }
                else return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "invalid login credentials"
                });
            }

            return BadRequest(new
            {
                StatusCode = 400,
                Message = "request object is not a valid credential"
            });
        }

        private async Task<List<Claim>> GetValidClaims(LiteForumUser user) {
            IdentityOptions _options = new IdentityOptions();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64),
                new Claim(_options.ClaimsIdentity.UserIdClaimType, user.Id.ToString()),
                new Claim(_options.ClaimsIdentity.UserNameClaimType, user.UserName)
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            claims.AddRange(userClaims);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (Claim roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }
            return claims;
        }

        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        private void AddErrors(IdentityResult result) {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
