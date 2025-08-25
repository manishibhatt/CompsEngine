using System;
using System.Collections.Generic;

namespace CompsEngine.Data.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public ulong EmailConfirmed { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<EmailVerification> EmailVerifications { get; set; } = new List<EmailVerification>();

    public virtual ICollection<PasswordReset> PasswordResets { get; set; } = new List<PasswordReset>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
