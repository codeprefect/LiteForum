
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleForum.Models;

namespace SimpleForum.Helpers {
    public static class StartupHelper
    {
        private static RoleManager<IdentityRole> _roleManager;
        private static UserManager<SimpleForumUser> _userManager;
        private static IServiceProvider _serviceProvider;
        private static IConfiguration _configuration;

        public static async Task CreateRolesAndAdminUser(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            _userManager = serviceProvider.GetRequiredService<UserManager<SimpleForumUser>>();
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            await CreateRoles();
            await CreateAdminUser();
        }

        private static async Task CreateRoles()
        {
            //adding customs roles
            string[] roleNames = { AppConstants.Roles.Admin, AppConstants.Roles.Member };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                //creating the roles and seeding them to the database
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task CreateAdminUser()
        {
            //creating an admin user who could maintain the web app
            var adminUser = new SimpleForumUser
            {
                UserName = _configuration.GetSection("AppSettings")["AdminUsername"],
                Email = _configuration.GetSection("AppSettings")["AdminEmail"]
            };

            string userPassword = _configuration.GetSection("AppSettings")["AdminPassword"];
            var user = await _userManager.FindByEmailAsync(adminUser.Email);

            if (user == null)
            {
                var createAdminUser = await _userManager.CreateAsync(adminUser, userPassword);
                if (createAdminUser.Succeeded)
                {
                    //here we tie the new user to the "Admin" role 
                    await _userManager.AddToRoleAsync(adminUser, AppConstants.Roles.Admin);
                }
            }
        }
    }
}