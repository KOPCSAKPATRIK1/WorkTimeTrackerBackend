using System;
using System.Collections.Generic;

namespace WorkTimeTracker.Core.Models.Domain;

public partial class WorkTask
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? AssignedToUserId { get; set; }

    public virtual User? AssignedToUser { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}
