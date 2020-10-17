using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Areas.Identity.Data;
using BugTracker.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BugTracker.Configuration
{
    public class IdentityDataInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityDataInitializer(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async void Seed()
        {
            await CreateAdminRole();
            await CreateManagerRole();
            await CreateDeveloperRole();
            await CreateViewerRole();
            //await CreateDemoAdminRole();
            //await CreateDemoManagerRole();
            //await CreateDemoManager();
        }

        private async Task CreateAdminRole()
        {
            if ((await _roleManager.FindByNameAsync("Admin")) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
            }
        }

        private async Task CreateManagerRole()
        {
            if ((await _roleManager.FindByNameAsync("Manager")) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "Manager" });
            }
        }

        private async Task CreateDeveloperRole()
        {
            if ((await _roleManager.FindByNameAsync("Developer")) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "Developer" });
            }
        }
        private async Task CreateViewerRole()
        {
            if ((await _roleManager.FindByNameAsync("Viewer")) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "Viewer" });
            }
        }

        private async Task CreateDemoAdminRole()
        {
            if ((await _roleManager.FindByNameAsync("DemoAdmin")) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "DemoAdmin" });
            }
        }

        private async Task CreateDemoManagerRole()
        {
            if ((await _roleManager.FindByNameAsync("DemoManager")) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "DemoManager" });
            }
        }

        private async Task CreateDemoManager()
        {
            if ((await _userManager.FindByNameAsync("Demo Manager")) == null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = "Demo Manager",
                    Email = "Demo Manager"
                };

                IdentityResult result = _userManager.CreateAsync(user).Result;

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "DemoManager");

                }
            }
        }

    }
}
