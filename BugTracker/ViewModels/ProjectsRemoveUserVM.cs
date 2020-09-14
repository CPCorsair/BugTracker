using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
    public class ProjectsRemoveUserVM
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public string UserId { get; set; }
        public SelectList CurrentUsers { get; set; }
    }
}
