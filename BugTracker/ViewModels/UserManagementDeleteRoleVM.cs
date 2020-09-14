using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
    public class UserManagementDeleteRoleVM
    {
        public string UserId { get; set; }
        public string OldRole { get; set; }
        public SelectList CurrentRoles { get; set; }
        public string Email { get; set; }
    }
}
