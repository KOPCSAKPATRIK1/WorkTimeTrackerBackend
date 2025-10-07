using System;
using System.Collections.Generic;

namespace WorkTimeTracker.Core.Models.Domain;

public partial class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? ParentProjectId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual ICollection<Project> InverseParentProject { get; set; } = new List<Project>();

    public virtual Project? ParentProject { get; set; }

    public virtual ICollection<WorkTask> Tasks { get; set; } = new List<WorkTask>();

    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}
