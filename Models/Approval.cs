namespace AISmartSheet.API.Models;

public enum ApprovalStatus
{
    pending,
    approved,
    rejected
}

public class Approval
{
    public Guid Id { get; set; }
    public Guid TimeEntryId { get; set; }
    public Guid ApproverId { get; set; }
    public ApprovalStatus Status { get; set; } = ApprovalStatus.pending;
    public string? Comments { get; set; }
    public int ApprovalLevel { get; set; } = 1; // 1 = Manager, 2 = Admin
    public bool IsFinalApproval { get; set; } = false;
    public Guid? AdminApproverId { get; set; }
    public DateTime? AdminApprovalDate { get; set; }
    public string? AdminComments { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public TimeEntry? TimeEntry { get; set; }
    public User? Approver { get; set; }
    public User? AdminApprover { get; set; }
}
