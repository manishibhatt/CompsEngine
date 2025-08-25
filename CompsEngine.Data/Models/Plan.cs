using System;
using System.Collections.Generic;

namespace CompsEngine.Data.Models;

public partial class Plan
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int PriceCents { get; set; }

    public string Currency { get; set; } = null!;

    public string BillInterval { get; set; } = null!;

    public int TrialDays { get; set; }

    public ulong IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
