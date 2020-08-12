using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BugTracker.Configuration
{
    public class UserRoleSeed
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserRoleSeed(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async void Seed()
        {
            await CreateAdminRole();
            await CreateManagerRole();
            await CreateDeveloperRole();
            await CreateViewerRole();
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
    }
}
