using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int ProjectId {get; set;}
        public string Title { get; set; }
        public DateTime DateCreated { get; set;}
        public string Description { get; set; }
    }
}
