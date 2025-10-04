namespace AISmartSheet.API.Models;

public class ProjectManager
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid ManagerId { get; set; }
    public Guid AssignedBy { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Project? Project { get; set; }
    public User? Manager { get; set; }
    public User? AssignedByUser { get; set; }
}
