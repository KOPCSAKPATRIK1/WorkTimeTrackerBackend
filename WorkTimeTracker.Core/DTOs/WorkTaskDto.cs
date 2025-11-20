using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTimeTracker.Core.Models.Domain;

namespace WorkTimeTracker.Core.DTOs
{
    public class WorkTaskDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
