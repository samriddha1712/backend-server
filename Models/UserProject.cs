namespace AISmartSheet.API.Models;

public class UserProject
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? AssignedBy { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public Guid? RemovedBy { get; set; }
    public DateTime? RemovedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? TeamId { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public Project? Project { get; set; }
    public User? AssignedByUser { get; set; }
    public User? RemovedByUser { get; set; }
}
