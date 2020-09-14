using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Identity;
using BugTracker.Areas.Identity.Data;
using System.Security.Claims;
using BugTracker.ViewModels;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private readonly BugTrackerContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthDbContext _dbContext;

        public TicketsController(BugTrackerContext context, 
                                 UserManager<ApplicationUser> userManager,
                                 AuthDbContext dbContext)
        {
            _context = context;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tickets.ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create()
        {
            //PopulateProjectsDropDownList();
            //select all projects, order by title
            var projectsQuery = from d in _context.Projects
                                orderby d.Title
                                select d;
            var allDevelopers = await _userManager.GetUsersInRoleAsync("Developer");

            var vm = new TicketsCreateOrEditVM { 
                Projects = new SelectList(projectsQuery, "Id", "Title"),
                Developers = new SelectList(allDevelopers,"Id", "Email")
        };


            return View(vm);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketsCreateOrEditVM rvm)
        {
            if (ModelState.IsValid)
            {
               // rvm.ticket.SubmitterId = _userManager.GetUserId(User);

                Ticket t = new Ticket
                {
                    Id = rvm.ticket.Id,
                    ProjectId = rvm.ticket.ProjectId,
                    Title = rvm.ticket.Title,
                    Description = rvm.ticket.Description,
                    SubmitterId = _userManager.GetUserId(User),
                    DateCreated = rvm.ticket.DateCreated,
                    DeveloperId = rvm.ticket.DeveloperId,
                    Priority = rvm.ticket.Priority,
                    Severity = rvm.ticket.Severity,
                    Status = rvm.ticket.Status
                };
                
                _context.Add(t);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            //PopulateProjectsDropDownList();
            //select all projects, order by title
            var projectsQuery = from d in _context.Projects
                                orderby d.Title
                                select d;
            var allDevelopers = await _userManager.GetUsersInRoleAsync("Developer");

            var vm = new TicketsCreateOrEditVM
            {
                Projects = new SelectList(projectsQuery, "Id", "Title"),
                Developers = new SelectList(allDevelopers, "Id", "Email")
            };

            return View(vm);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            //PopulateProjectsDropDownList();

            //select all projects, order by title
            var projectsQuery = from d in _context.Projects
                                orderby d.Title
                                select d;
            var allDevelopers = await _userManager.GetUsersInRoleAsync("Developer");

            var vm = new TicketsCreateOrEditVM
            {
                Projects = new SelectList(projectsQuery, "Id", "Title"),
                Developers = new SelectList(allDevelopers, "Id", "Email"),
                ticket = ticket
            };
            return View(vm);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TicketsCreateOrEditVM rvm)
        {
            if (id != rvm.ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Ticket t = new Ticket
                {
                    Id = rvm.ticket.Id,
                    ProjectId = rvm.ticket.ProjectId,
                    Title = rvm.ticket.Title,
                    Description = rvm.ticket.Description,
                    SubmitterId = _userManager.GetUserId(User),
                    DateCreated = rvm.ticket.DateCreated,
                    DeveloperId = rvm.ticket.DeveloperId,
                    Priority = rvm.ticket.Priority,
                    Severity = rvm.ticket.Severity,
                    Status = rvm.ticket.Status
                };

                try
                {
                    _context.Update(t);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(rvm.ticket.Id))
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

            var projectsQuery = from d in _context.Projects
                                orderby d.Title
                                select d;
            var allDevelopers = await _userManager.GetUsersInRoleAsync("Developer");

            var vm = new TicketsCreateOrEditVM
            {
                Projects = new SelectList(projectsQuery, "Id", "Title"),
                Developers = new SelectList(allDevelopers, "Id", "Email"),
                ticket = rvm.ticket
            };

            return View(vm);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }

        private void PopulateProjectsDropDownList()
        {
            //select all projects, order by title
            var projectsQuery = from d in _context.Projects
                                orderby d.Title
                                select d;
            ViewBag.ProjectId = new SelectList(projectsQuery, "Id", "Title");
        }
    }
}
