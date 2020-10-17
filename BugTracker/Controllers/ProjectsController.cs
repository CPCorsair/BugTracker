using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BugTracker.Areas.Identity.Data;
using BugTracker.Authorization;
using BugTracker.ViewModels;
using System.Security.Policy;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System.Security.Claims;

namespace BugTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly BugTrackerContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthDbContext _dbContext;
        private readonly ClaimsPrincipal _claimsPrincipal;

        public ProjectsController(BugTrackerContext context, 
                                  IAuthorizationService authorizationService,
                                  UserManager<ApplicationUser> userManager,
                                  AuthDbContext dbContext,
                                  ClaimsPrincipal claimsPrincipal)
        {
            _context = context;
            _authorizationService = authorizationService;
            _userManager = userManager;
            _dbContext = dbContext;
            _claimsPrincipal = claimsPrincipal;
        }

        // GET: Projects
        public async Task<IActionResult> Index(string searchString)
        {
            var projects = from p in _context.Projects
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(s => s.Title.Contains(searchString));
            }
            return View(await projects.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include (m => m.Tickets) //Project model already has Tickets associated (icollection)
                .AsNoTracking() //optional; more efficient way to do things apparently
                .FirstOrDefaultAsync(m => m.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, project, ProjectOperations.Read);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            var vm = new ProjectsDetailsVM
            {
                Project = project,
                UsersWithClaim = await _userManager.GetUsersForClaimAsync(new Claim("AssignedProject", project.Id.ToString()))
            };

            return View(vm);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,DateCreated")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.OwnerId = _userManager.GetUserId(User);

                //var isAuthorized = await _authorizationService.AuthorizeAsync(User, project, ProjectOperations.Create);

                //if (!isAuthorized.Succeeded)
                //{
                //    return Forbid();
                //}

                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, project, ProjectOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //NOTE: int id is mostly likely passed in through the URL
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OwnerId,Title,Description,DateCreated")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            var uneditedProject = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (uneditedProject.OwnerId != project.OwnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, project, ProjectOperations.Update);

                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }

                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, project, ProjectOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return View(project);
        }

        //do this
        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, project, ProjectOperations.Delete);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            var projectUsers = await _userManager.GetUsersForClaimAsync(new Claim("AssignedProject", project.Id.ToString()));
            
            //if projectUsers is not empty, delete the user claims associated with project
            if (!projectUsers.Any())
            {
                foreach(ApplicationUser u in projectUsers)
                {
                    await _userManager.RemoveClaimAsync(u, new Claim("AssignedProject", project.Id.ToString()));
                }
            }

            _context.Tickets.Where(t => t.ProjectId == project.Id).ToList().ForEach(t => _context.Tickets.Remove(t));

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> AddUser(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //getting the project using id
            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            //var usersOnProject = await _userManager.GetUsersForClaimAsync(new Claim("AssignedProject", project.Id.ToString()));

            var vm = new ProjectsAddUserVM
            {
                ProjectId = project.Id,
                ProjectTitle = project.Title,
                Users = new SelectList(_dbContext.Users.OrderBy(u => u.Email), "Id", "Email")
                
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(ProjectsAddUserVM rvm)
        {
            var user = await _userManager.FindByIdAsync(rvm.UserId);

            //setting values in view model in case there is an error and the view has to be returned again
            var project = await _context.Projects.FirstOrDefaultAsync(m => m.Id == rvm.ProjectId);
            rvm.Users = new SelectList(_dbContext.Users.OrderBy(u => u.Email), "Id", "Email");
            rvm.ProjectId = project.Id;
            rvm.ProjectTitle = project.Title;


            if (ModelState.IsValid)
            {
                //checking if there are duplicate claims
                var userclaims = await _userManager.GetClaimsAsync(user);
                var userHasClaim = userclaims.Any(f => f.Type == "AssignedProject" && f.Value == project.Id.ToString());
                if (userHasClaim)
                {
                    ModelState.AddModelError(string.Empty, "User is already assigned to project");
                    
                    return View(rvm);
                }

                //adding the claim to the user
                var result = await _userManager.AddClaimAsync(user, new Claim("AssignedProject", project.Id.ToString()));
                if (result.Succeeded)
                {
                    return RedirectToAction("Details", new { id = rvm.ProjectId.ToString() });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }
            
            return View(rvm);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveUser(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
               .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            var usersExist = await _userManager.GetUsersForClaimAsync(new Claim("AssignedProject", project.Id.ToString()));
            if (!usersExist.Any())
            {
                ModelState.AddModelError(string.Empty, "There are no users assigned to the project");
                return RedirectToAction(nameof(NoUsersToRemove));
            }

            var vm = new ProjectsRemoveUserVM
            {
                ProjectId = project.Id,
                ProjectTitle = project.Title,
                CurrentUsers = 
                    new SelectList(await _userManager.GetUsersForClaimAsync(new Claim("AssignedProject", project.Id.ToString())), "Id", "Email")
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUser(ProjectsRemoveUserVM rvm)
        {
            var user = await _userManager.FindByIdAsync(rvm.UserId);
            var project = await _context.Projects.FirstOrDefaultAsync(m => m.Id == rvm.ProjectId);

            var result = await _userManager.RemoveClaimAsync(user, new Claim("AssignedProject", project.Id.ToString()));
            if (result.Succeeded)
            {
                return RedirectToAction("Details", new { id = rvm.ProjectId.ToString() });
            }

            //repopulating view model
            rvm.ProjectId = project.Id;
            rvm.ProjectTitle = project.Title;
            rvm.CurrentUsers = 
                    new SelectList(await _userManager.GetUsersForClaimAsync(new Claim("AssignedProject", project.Id.ToString())), "Id", "Email");

            return View(rvm);
        }

        [HttpGet]
        public ActionResult AddProjectTicket(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            return RedirectToAction("Create","Tickets");
        }


        public IActionResult NoUsersToRemove()
        {
            return View();
        }
    }
}
