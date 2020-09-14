using BugTracker.Areas.Identity.Data;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
    public class UserManagementIndexVM
    {
        public List<ApplicationUser> Users { get; set; }
    }
}
