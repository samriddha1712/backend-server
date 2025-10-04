namespace AISmartSheet.API.DTOs;

// Authentication DTOs
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "developer";
}

// User DTOs
public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "developer";
}

public class UpdateUserRequest
{
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
}

// Project DTOs
public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Client { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? HourlyRate { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Optional relations
    public UserDto? Creator { get; set; }
    public int? AssignedUsersCount { get; set; }
    public int? TotalHoursLogged { get; set; }
}

public class CreateProjectRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Client { get; set; }
    public string Status { get; set; } = "active";
    public decimal? HourlyRate { get; set; }
}

public class UpdateProjectRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Client { get; set; }
    public string? Status { get; set; }
    public decimal? HourlyRate { get; set; }
}

public class AssignUserToProjectRequest
{
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
}

// Time Entry DTOs
public class TimeEntryDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal Hours { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Optional relations
    public UserDto? User { get; set; }
    public ProjectDto? Project { get; set; }
}

public class CreateTimeEntryRequest
{
    public Guid ProjectId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal Hours { get; set; }
    public DateTime Date { get; set; }
}

public class UpdateTimeEntryRequest
{
    public Guid? ProjectId { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public decimal? Hours { get; set; }
    public DateTime? Date { get; set; }
}

public class BulkTimeEntryRequest
{
    public Guid ProjectId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Hours { get; set; }
    public List<DateTime> Dates { get; set; } = new();
}

// Approval DTOs
public class ApprovalDto
{
    public Guid Id { get; set; }
    public Guid TimeEntryId { get; set; }
    public Guid ApproverId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Comments { get; set; }
    public int ApprovalLevel { get; set; }
    public bool IsFinalApproval { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Optional relations
    public TimeEntryDto? TimeEntry { get; set; }
    public UserDto? Approver { get; set; }
}

public class ApproveTimeEntryRequest
{
    public Guid TimeEntryId { get; set; }
    public string? Comments { get; set; }
}

public class RejectTimeEntryRequest
{
    public Guid TimeEntryId { get; set; }
    public string Comments { get; set; } = string.Empty;
}

public class BulkApprovalRequest
{
    public List<Guid> TimeEntryIds { get; set; } = new();
    public string? Comments { get; set; }
}

// Dashboard DTOs
public class DashboardStatsDto
{
    public decimal TotalHoursToday { get; set; }
    public decimal TotalHoursWeek { get; set; }
    public decimal TotalHoursMonth { get; set; }
    public int ActiveProjects { get; set; }
    public int PendingApprovals { get; set; }
    
    // Manager-specific
    public int? ManagedProjects { get; set; }
    public int? TeamMembers { get; set; }
    public int? PendingManagerApprovals { get; set; }
    
    // Admin-specific
    public int? TotalUsers { get; set; }
    public int? TotalProjects { get; set; }
}

public class ProjectStatsDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public decimal TotalHours { get; set; }
    public int TotalEntries { get; set; }
    public DateTime? LastEntry { get; set; }
}

// Generic Response Wrapper
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}

public class PagedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
