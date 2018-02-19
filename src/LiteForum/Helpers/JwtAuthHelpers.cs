using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace LiteForum.Helpers
{
    public static class JwtAuthHelper
    {
        public static AuthenticationBuilder ConfigureJwtAuth(this AuthenticationBuilder auth, IConfiguration config)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Tokens:Secret"])),
                ValidIssuer = config["Tokens:Issuer"],
                ValidAudience = config["Tokens:Audience"],
            };

            return auth.AddJwtBearer("JwtBearer", options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = tokenValidationParameters;
            });
        }
    }
}
