namespace AISmartSheet.API.Models;

public enum TimeEntryStatus
{
    draft,
    submitted,
    manager_approved,
    approved,
    rejected
}

public class TimeEntry
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal Hours { get; set; }
    public DateTime Date { get; set; }
    public TimeEntryStatus Status { get; set; } = TimeEntryStatus.draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? User { get; set; }
    public Project? Project { get; set; }
    public ICollection<Approval> Approvals { get; set; } = new List<Approval>();
}
