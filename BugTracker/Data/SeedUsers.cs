using BugTracker.Areas.Identity.Data;
using BugTracker.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Data
{
    public class SeedUsers
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AuthDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AuthDbContext>>()))
            {
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything



                var adminID = await EnsureUser(serviceProvider, "DemoAdmin");
                await EnsureRole(serviceProvider, adminID, Constants.ProjectAdminsRole);

                var managerID = await EnsureUser(serviceProvider, "DemoManager");
                await EnsureRole(serviceProvider, managerID, Constants.ProjectManagersRole);

                var developer1ID = await EnsureUser(serviceProvider, "DemoDeveloper1");
                await EnsureRole(serviceProvider, developer1ID, Constants.ProjectDevelopersRole);

                var developer2ID = await EnsureUser(serviceProvider, "DemoDeveloper2");
                await EnsureRole(serviceProvider, developer2ID, Constants.ProjectDevelopersRole);

            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = UserName,
                    Email = UserName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }
    }
}
