using System;
using System.Collections.Generic;

namespace CompsEngine.Data.Models;

public partial class Subscription
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid PlanId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime StartAt { get; set; }

    public DateTime CurrentPeriodStart { get; set; }

    public DateTime CurrentPeriodEnd { get; set; }

    public DateTime? TrialEndAt { get; set; }

    public ulong CancelAtPeriodEnd { get; set; }

    public DateTime? CanceledAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Plan Plan { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
