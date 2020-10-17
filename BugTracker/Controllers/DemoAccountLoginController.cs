using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BugTracker.Areas.Identity.Data;

namespace BugTracker.Controllers
{
    public class DemoAccountLoginController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public DemoAccountLoginController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> DemoAccountLogin(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            await _signInManager.SignInAsync(user, true);
                return View();
        }
    }
}
