using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTimeTracker.Core.DTOs
{
    public class CreateWorkTaskRequest
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
