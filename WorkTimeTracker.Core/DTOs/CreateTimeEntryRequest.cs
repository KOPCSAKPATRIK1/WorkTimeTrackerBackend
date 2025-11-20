using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTimeTracker.Core.DTOs
{
    public class CreateTimeEntryRequest
    {
        public int UserId { get; set; }
        public int? ProjectId { get; set; }
        public int? TaskId { get; set; }
        public DateOnly Date { get; set; }
        public decimal Hours { get; set; }
        public string? Description { get; set; }
    }
}
