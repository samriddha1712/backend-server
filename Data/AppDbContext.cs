using AISmartSheet.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AISmartSheet.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<TimeEntry> TimeEntries { get; set; }
    public DbSet<UserProject> UserProjects { get; set; }
    public DbSet<Approval> Approvals { get; set; }
    public DbSet<ProjectManager> ProjectManagers { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(255);
            entity.Property(e => e.FullName).HasColumnName("full_name").IsRequired().HasMaxLength(255);
            entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").IsRequired();
            entity.Property(e => e.Role).HasColumnName("role")
                .HasConversion<string>()
                .IsRequired();
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.AddedBy).HasColumnName("added_by");
            entity.Property(e => e.DeactivatedBy).HasColumnName("deactivated_by");
            entity.Property(e => e.DeactivatedAt).HasColumnName("deactivated_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");

            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasOne(e => e.AddedByUser)
                .WithMany()
                .HasForeignKey(e => e.AddedBy)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.DeactivatedByUser)
                .WithMany()
                .HasForeignKey(e => e.DeactivatedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Project entity
        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("projects");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Client).HasColumnName("client");
            entity.Property(e => e.Status).HasColumnName("status")
                .HasConversion<string>()
                .HasDefaultValue(ProjectStatus.active);
            entity.Property(e => e.HourlyRate).HasColumnName("hourly_rate").HasPrecision(10, 2);
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");

            entity.HasOne(e => e.Creator)
                .WithMany(u => u.CreatedProjects)
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure TimeEntry entity
        modelBuilder.Entity<TimeEntry>(entity =>
        {
            entity.ToTable("time_entries");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Description).HasColumnName("description").IsRequired();
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.Hours).HasColumnName("hours").HasPrecision(5, 2).IsRequired();
            entity.Property(e => e.Date).HasColumnName("date").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status")
                .HasConversion<string>()
                .HasDefaultValue(TimeEntryStatus.draft);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");

            entity.HasIndex(e => new { e.UserId, e.Date });
            entity.HasIndex(e => new { e.ProjectId, e.Date });

            entity.HasOne(e => e.User)
                .WithMany(u => u.TimeEntries)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Project)
                .WithMany(p => p.TimeEntries)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure UserProject entity
        modelBuilder.Entity<UserProject>(entity =>
        {
            entity.ToTable("user_projects");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.AssignedBy).HasColumnName("assigned_by");
            entity.Property(e => e.AssignedAt).HasColumnName("assigned_at").HasDefaultValueSql("NOW()");
            entity.Property(e => e.RemovedBy).HasColumnName("removed_by");
            entity.Property(e => e.RemovedAt).HasColumnName("removed_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.TeamId).HasColumnName("team_id");

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => new { e.UserId, e.ProjectId, e.TeamId });

            entity.HasOne(e => e.User)
                .WithMany(u => u.UserProjects)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Project)
                .WithMany(p => p.UserProjects)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.AssignedByUser)
                .WithMany()
                .HasForeignKey(e => e.AssignedBy)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.RemovedByUser)
                .WithMany()
                .HasForeignKey(e => e.RemovedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Approval entity
        modelBuilder.Entity<Approval>(entity =>
        {
            entity.ToTable("approvals");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TimeEntryId).HasColumnName("time_entry_id");
            entity.Property(e => e.ApproverId).HasColumnName("approver_id");
            entity.Property(e => e.Status).HasColumnName("status")
                .HasConversion<string>()
                .HasDefaultValue(ApprovalStatus.pending);
            entity.Property(e => e.Comments).HasColumnName("comments");
            entity.Property(e => e.ApprovalLevel).HasColumnName("approval_level").HasDefaultValue(1);
            entity.Property(e => e.IsFinalApproval).HasColumnName("is_final_approval").HasDefaultValue(false);
            entity.Property(e => e.AdminApproverId).HasColumnName("admin_approver_id");
            entity.Property(e => e.AdminApprovalDate).HasColumnName("admin_approval_date");
            entity.Property(e => e.AdminComments).HasColumnName("admin_comments");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");

            entity.HasIndex(e => e.TimeEntryId);
            entity.HasIndex(e => e.ApproverId);

            entity.HasOne(e => e.TimeEntry)
                .WithMany(t => t.Approvals)
                .HasForeignKey(e => e.TimeEntryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Approver)
                .WithMany(u => u.Approvals)
                .HasForeignKey(e => e.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AdminApprover)
                .WithMany()
                .HasForeignKey(e => e.AdminApproverId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure ProjectManager entity
        modelBuilder.Entity<ProjectManager>(entity =>
        {
            entity.ToTable("project_managers");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.AssignedBy).HasColumnName("assigned_by");
            entity.Property(e => e.AssignedAt).HasColumnName("assigned_at").HasDefaultValueSql("NOW()");

            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.ManagerId);
            entity.HasIndex(e => new { e.ProjectId, e.ManagerId }).IsUnique();

            entity.HasOne(e => e.Project)
                .WithMany(p => p.ProjectManagers)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Manager)
                .WithMany(u => u.ProjectManagers)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.AssignedByUser)
                .WithMany()
                .HasForeignKey(e => e.AssignedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure AuditLog entity
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("audit_logs");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Action).HasColumnName("action").IsRequired().HasMaxLength(255);
            entity.Property(e => e.ResourceType).HasColumnName("resource_type").IsRequired().HasMaxLength(100);
            entity.Property(e => e.ResourceId).HasColumnName("resource_id");
            entity.Property(e => e.Details).HasColumnName("details");
            entity.Property(e => e.IpAddress).HasColumnName("ip_address");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreatedAt);

            entity.HasOne(e => e.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
