using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTimeTracker.Core.DTOs
{
    public class TimeEntryDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public int? TaskId { get; set; }
        public string? TaskName { get; set; }
        public DateOnly Date { get; set; }
        public decimal Hours { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
