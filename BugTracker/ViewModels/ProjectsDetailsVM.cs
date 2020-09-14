using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Areas.Identity.Data;
using BugTracker.Models;

namespace BugTracker.ViewModels
{
    public class ProjectsDetailsVM
    {
        public Project Project { get; set; }

        //stores users who have claim indicating they're assigned to this project

        public IList<ApplicationUser> UsersWithClaim { get; set; } 
    }
}
