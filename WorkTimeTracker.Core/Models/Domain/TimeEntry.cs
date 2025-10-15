using System;
using System.Collections.Generic;

namespace WorkTimeTracker.Core.Models.Domain;

public partial class TimeEntry
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? ProjectId { get; set; }

    public int? TaskId { get; set; }

    public DateOnly Date { get; set; }

    public decimal Hours { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Project? Project { get; set; } = null!;

    public virtual WorkTask? Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
