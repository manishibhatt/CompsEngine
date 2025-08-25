using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace CompsEngine.Data.Models;

public partial class CompsEngineDbContext : DbContext
{
    public CompsEngineDbContext()
    {
    }

    public CompsEngineDbContext(DbContextOptions<CompsEngineDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EmailVerification> EmailVerifications { get; set; }

    public virtual DbSet<PasswordReset> PasswordResets { get; set; }

    public virtual DbSet<Plan> Plans { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=compsengine_db;user=root;password=Admin@1234", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.43-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<EmailVerification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("email_verifications");

            entity.HasIndex(e => e.Token, "uq_email_verifications_token").IsUnique();

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasMaxLength(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt)
                .HasMaxLength(6)
                .HasColumnName("expires_at");
            entity.Property(e => e.Token)
                .HasMaxLength(200)
                .HasColumnName("token");
            entity.Property(e => e.UsedAt)
                .HasMaxLength(6)
                .HasColumnName("used_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.EmailVerifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("email_verifications_ibfk_1");
        });

        modelBuilder.Entity<PasswordReset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("password_resets");

            entity.HasIndex(e => e.Token, "uq_password_resets_token").IsUnique();

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasMaxLength(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt)
                .HasMaxLength(6)
                .HasColumnName("expires_at");
            entity.Property(e => e.Token)
                .HasMaxLength(200)
                .HasColumnName("token");
            entity.Property(e => e.UsedAt)
                .HasMaxLength(6)
                .HasColumnName("used_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("password_resets_ibfk_1");
        });

        modelBuilder.Entity<Plan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("plans");

            entity.HasIndex(e => e.Code, "code").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BillInterval)
                .HasMaxLength(10)
                .HasColumnName("bill_interval");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasMaxLength(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                .HasColumnName("created_at");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .HasDefaultValueSql("'USD'")
                .IsFixedLength()
                .HasColumnName("currency");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PriceCents).HasColumnName("price_cents");
            entity.Property(e => e.TrialDays).HasColumnName("trial_days");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("refresh_tokens");

            entity.HasIndex(e => e.Token, "ix_refresh_tokens_token");

            entity.HasIndex(e => e.UserId, "ix_refresh_tokens_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasMaxLength(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt)
                .HasMaxLength(6)
                .HasColumnName("expires_at");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .HasColumnName("ip_address");
            entity.Property(e => e.RevokedAt)
                .HasMaxLength(6)
                .HasColumnName("revoked_at");
            entity.Property(e => e.Token)
                .HasMaxLength(200)
                .HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("refresh_tokens_ibfk_1");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("subscriptions");

            entity.HasIndex(e => e.Status, "ix_subscriptions_status");

            entity.HasIndex(e => e.UserId, "ix_subscriptions_user");

            entity.HasIndex(e => e.PlanId, "plan_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CancelAtPeriodEnd)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("cancel_at_period_end");
            entity.Property(e => e.CanceledAt)
                .HasMaxLength(6)
                .HasColumnName("canceled_at");
            entity.Property(e => e.CreatedAt)
                .HasMaxLength(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrentPeriodEnd)
                .HasMaxLength(6)
                .HasColumnName("current_period_end");
            entity.Property(e => e.CurrentPeriodStart)
                .HasMaxLength(6)
                .HasColumnName("current_period_start");
            entity.Property(e => e.PlanId).HasColumnName("plan_id");
            entity.Property(e => e.StartAt)
                .HasMaxLength(6)
                .HasColumnName("start_at");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.TrialEndAt)
                .HasMaxLength(6)
                .HasColumnName("trial_end_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Plan).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("subscriptions_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("subscriptions_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasMaxLength(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .HasColumnName("email");
            entity.Property(e => e.EmailConfirmed)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("email_confirmed");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(64)
                .HasColumnName("password_hash");
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(16)
                .HasColumnName("password_salt");
            entity.Property(e => e.UpdatedAt)
                .HasMaxLength(6)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .HasColumnName("user_name");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("user_roles_ibfk_2"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("user_roles_ibfk_1"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("user_roles");
                        j.HasIndex(new[] { "RoleId" }, "role_id");
                        j.IndexerProperty<Guid>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<Guid>("RoleId").HasColumnName("role_id");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
