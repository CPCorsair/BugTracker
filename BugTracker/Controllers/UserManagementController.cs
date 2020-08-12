using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Areas.Identity.Data;
using BugTracker.Data;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly AuthDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        //getting database context b/c want to display list of users
        public UserManagementController(AuthDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var vm = new UserManagementIndexViewModel
            {
                Users = _dbContext.Users.OrderBy(u => u.Email).ToList()
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> AddRole(string id)
        {
            var user = await GetUserById(id);
            var vm = new UserManagementAddRoleViewModel
            {
                Roles = GetAllRoles(),
                UserId = id,
                Email = user.Email
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(UserManagementAddRoleViewModel rvm) //rvm = result view model
        {
            var user = await GetUserById(rvm.UserId);

            if (ModelState.IsValid)
            {
                var result = await _userManager.AddToRoleAsync(user, rvm.NewRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }
            rvm.Email = user.Email;
            rvm.Roles = GetAllRoles();
            return View(rvm);
        }

        private async Task<ApplicationUser> GetUserById(string id) =>
            await _userManager.FindByIdAsync(id);

        private SelectList GetAllRoles() => new SelectList(_roleManager.Roles.OrderBy(r => r.Name));
    }
}
