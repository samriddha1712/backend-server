namespace AISmartSheet.API.Models;

public enum UserRole
{
    admin,
    manager,
    developer
}

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.developer;
    public bool IsActive { get; set; } = true;
    public Guid? AddedBy { get; set; }
    public Guid? DeactivatedBy { get; set; }
    public DateTime? DeactivatedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? AddedByUser { get; set; }
    public User? DeactivatedByUser { get; set; }
    public ICollection<Project> CreatedProjects { get; set; } = new List<Project>();
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    public ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();
    public ICollection<ProjectManager> ProjectManagers { get; set; } = new List<ProjectManager>();
    public ICollection<Approval> Approvals { get; set; } = new List<Approval>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
