using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTimeTracker.Core.DTOs
{
    public class CreateProjectRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? ParentProjectId { get; set; }
        public int? AssignedToUserId { get; set; }
    }

}
