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
        [Display(Name ="Project")]
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [Display(Name ="Submitter")]
        public string SubmitterId { get; set; }
        [Display(Name ="Created")]
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }
        [Display(Name ="Developer")]
        public string DeveloperId { get; set; }
        public TicketPriority Priority { get; set; }
        public TicketSeverity Severity { get; set; }
        public TicketStatus Status { get; set; }
    
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

    public enum TicketStatus
    {
        Ongoing,
        Resolved
    }
}
