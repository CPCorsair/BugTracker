using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Submitter { get; set; }
        [Display(Name ="Date Created")]
        public DateTime DateCreated { get; set; }
        public string Developer { get; set; }
        public TicketPriority Priority { get; set; }
        public TicketSeverity Severity { get; set; }
    
    }

    public enum TicketPriority
    {
        Low,
        Medium,
        High,
        Immediate
    }

    public enum TicketSeverity
    {
        Low,
        Minor,
        Major,
        Critical
    }
}
