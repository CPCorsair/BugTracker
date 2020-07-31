using BugTracker.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [Display(Name ="Date Created")]
        public DateTime DateCreated { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
