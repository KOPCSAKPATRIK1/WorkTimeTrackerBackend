using System;
using System.Collections.Generic;

namespace WorkTimeTracker.Core.Models.Domain;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Project> ProjectAssignedToUsers { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectCreatedByUsers { get; set; } = new List<Project>();

    public virtual ICollection<WorkTask> Tasks { get; set; } = new List<WorkTask>();

    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}
