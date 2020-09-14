using BugTracker.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
    public class UserManagementAddRoleVM
    {
        public string UserId { get; set; }
        public string NewRole { get; set; }
        public SelectList Roles { get; set; }
        public string Email { get; set; }
    }
}
