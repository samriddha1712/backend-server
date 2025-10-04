namespace AISmartSheet.API.Models;

public enum ProjectStatus
{
    active,
    inactive,
    completed
}

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Client { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.active;
    public decimal? HourlyRate { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? Creator { get; set; }
    public ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    public ICollection<ProjectManager> ProjectManagers { get; set; } = new List<ProjectManager>();
}
