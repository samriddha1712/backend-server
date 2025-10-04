using AISmartSheet.API.DTOs;
using AISmartSheet.API.Models;

namespace AISmartSheet.API.Services;

public interface IUserService
{
    Task<PagedResponse<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 20, string? search = null, string? role = null);
    Task<UserDto?> GetUserByIdAsync(Guid userId);
    Task<UserDto?> CreateUserAsync(CreateUserRequest request, Guid createdBy);
    Task<UserDto?> UpdateUserAsync(Guid userId, UpdateUserRequest request, Guid updatedBy);
    Task<bool> DeleteUserAsync(Guid userId, Guid deletedBy);
    Task<bool> DeactivateUserAsync(Guid userId, Guid deactivatedBy);
    Task<bool> ActivateUserAsync(Guid userId, Guid activatedBy);
}

public interface IProjectService
{
    Task<PagedResponse<ProjectDto>> GetAllProjectsAsync(int page = 1, int pageSize = 20, string? search = null, string? status = null);
    Task<List<ProjectDto>> GetUserProjectsAsync(Guid userId);
    Task<List<ProjectDto>> GetManagerProjectsAsync(Guid managerId);
    Task<ProjectDto?> GetProjectByIdAsync(Guid projectId);
    Task<ProjectDto?> CreateProjectAsync(CreateProjectRequest request, Guid createdBy);
    Task<ProjectDto?> UpdateProjectAsync(Guid projectId, UpdateProjectRequest request, Guid updatedBy);
    Task<bool> DeleteProjectAsync(Guid projectId);
    Task<bool> AssignUserToProjectAsync(Guid projectId, Guid userId, Guid assignedBy);
    Task<bool> RemoveUserFromProjectAsync(Guid projectId, Guid userId, Guid removedBy);
    Task<bool> AssignManagerToProjectAsync(Guid projectId, Guid managerId, Guid assignedBy);
    Task<bool> RemoveManagerFromProjectAsync(Guid projectId, Guid managerId);
    Task<List<UserDto>> GetProjectMembersAsync(Guid projectId);
}

public interface ITimeEntryService
{
    Task<PagedResponse<TimeEntryDto>> GetTimeEntriesAsync(
        Guid? userId = null,
        Guid? projectId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? status = null,
        int page = 1,
        int pageSize = 20);
    Task<TimeEntryDto?> GetTimeEntryByIdAsync(Guid timeEntryId);
    Task<TimeEntryDto?> CreateTimeEntryAsync(CreateTimeEntryRequest request, Guid userId);
    Task<List<TimeEntryDto>> CreateBulkTimeEntriesAsync(BulkTimeEntryRequest request, Guid userId);
    Task<TimeEntryDto?> UpdateTimeEntryAsync(Guid timeEntryId, UpdateTimeEntryRequest request, Guid userId);
    Task<bool> DeleteTimeEntryAsync(Guid timeEntryId, Guid userId);
    Task<bool> SubmitTimeEntryAsync(Guid timeEntryId, Guid userId);
    Task<bool> SubmitMultipleTimeEntriesAsync(List<Guid> timeEntryIds, Guid userId);
    Task<decimal> GetTotalHoursAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
}

public interface IApprovalService
{
    Task<PagedResponse<TimeEntryDto>> GetPendingApprovalsAsync(
        Guid? managerId = null,
        Guid? projectId = null,
        int page = 1,
        int pageSize = 20);
    Task<bool> ManagerApproveTimeEntryAsync(Guid timeEntryId, Guid managerId, string? comments = null);
    Task<bool> AdminApproveTimeEntryAsync(Guid timeEntryId, Guid adminId, string? comments = null);
    Task<bool> RejectTimeEntryAsync(Guid timeEntryId, Guid approverId, string comments);
    Task<bool> BulkApproveTimeEntriesAsync(List<Guid> timeEntryIds, Guid approverId, string? comments = null);
    Task<List<ApprovalDto>> GetApprovalHistoryAsync(Guid timeEntryId);
}

public interface IDashboardService
{
    Task<DashboardStatsDto> GetUserDashboardStatsAsync(Guid userId);
    Task<DashboardStatsDto> GetManagerDashboardStatsAsync(Guid managerId);
    Task<DashboardStatsDto> GetAdminDashboardStatsAsync();
    Task<List<ProjectStatsDto>> GetUserProjectStatsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<List<ProjectStatsDto>> GetManagerProjectStatsAsync(Guid managerId, DateTime? startDate = null, DateTime? endDate = null);
    Task<List<ProjectStatsDto>> GetAllProjectStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
}

public interface IAuditLogService
{
    Task LogActionAsync(Guid userId, string action, string resourceType, Guid? resourceId = null, string? details = null, string? ipAddress = null);
    Task<PagedResponse<AuditLog>> GetAuditLogsAsync(
        Guid? userId = null,
        string? action = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int page = 1,
        int pageSize = 50);
}
