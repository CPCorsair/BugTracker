using BugTracker.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
    public class TicketsCreateOrEditVM
    {
        public SelectList Projects { get; set; }
        public SelectList Developers { get; set; }
        public Ticket ticket { get; set; }
    }
}
