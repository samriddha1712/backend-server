using AISmartSheet.API.Data;
using AISmartSheet.API.DTOs;
using AISmartSheet.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AISmartSheet.API.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(AppDbContext context, ILogger<ProjectService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResponse<ProjectDto>> GetAllProjectsAsync(int page = 1, int pageSize = 20, string? search = null, string? status = null)
    {
        var query = _context.Projects.AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Name.Contains(search) || (p.Client != null && p.Client.Contains(search)));

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ProjectStatus>(status, true, out var projectStatus))
            query = query.Where(p => p.Status == projectStatus);

        var totalCount = await query.CountAsync();
        var projects = await query
            .Include(p => p.Creator)
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<ProjectDto>
        {
            Data = projects.Select(MapToDto).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<List<ProjectDto>> GetUserProjectsAsync(Guid userId)
    {
        var projects = await _context.UserProjects
            .Where(up => up.UserId == userId && up.IsActive)
            .Include(up => up.Project)
            .ThenInclude(p => p!.Creator)
            .Select(up => up.Project!)
            .ToListAsync();

        return projects.Select(MapToDto).ToList();
    }

    public async Task<List<ProjectDto>> GetManagerProjectsAsync(Guid managerId)
    {
        var projects = await _context.ProjectManagers
            .Where(pm => pm.ManagerId == managerId)
            .Include(pm => pm.Project)
            .ThenInclude(p => p!.Creator)
            .Select(pm => pm.Project!)
            .ToListAsync();

        return projects.Select(MapToDto).ToList();
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(Guid projectId)
    {
        var project = await _context.Projects
            .Include(p => p.Creator)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        return project != null ? MapToDto(project) : null;
    }

    public async Task<ProjectDto?> CreateProjectAsync(CreateProjectRequest request, Guid createdBy)
    {
        if (!Enum.TryParse<ProjectStatus>(request.Status, true, out var projectStatus))
            projectStatus = ProjectStatus.active;

        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            Client = request.Client,
            Status = projectStatus,
            HourlyRate = request.HourlyRate,
            CreatedBy = createdBy
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return MapToDto(project);
    }

    public async Task<ProjectDto?> UpdateProjectAsync(Guid projectId, UpdateProjectRequest request, Guid updatedBy)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null) return null;

        if (!string.IsNullOrEmpty(request.Name))
            project.Name = request.Name;

        if (request.Description != null)
            project.Description = request.Description;

        if (request.Client != null)
            project.Client = request.Client;

        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<ProjectStatus>(request.Status, true, out var status))
            project.Status = status;

        if (request.HourlyRate.HasValue)
            project.HourlyRate = request.HourlyRate;

        project.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(project);
    }

    public async Task<bool> DeleteProjectAsync(Guid projectId)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null) return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignUserToProjectAsync(Guid projectId, Guid userId, Guid assignedBy)
    {
        var existing = await _context.UserProjects
            .FirstOrDefaultAsync(up => up.ProjectId == projectId && up.UserId == userId);

        if (existing != null)
        {
            if (!existing.IsActive)
            {
                existing.IsActive = true;
                existing.RemovedBy = null;
                existing.RemovedAt = null;
                await _context.SaveChangesAsync();
            }
            return true;
        }

        var userProject = new UserProject
        {
            UserId = userId,
            ProjectId = projectId,
            AssignedBy = assignedBy,
            IsActive = true
        };

        _context.UserProjects.Add(userProject);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveUserFromProjectAsync(Guid projectId, Guid userId, Guid removedBy)
    {
        var userProject = await _context.UserProjects
            .FirstOrDefaultAsync(up => up.ProjectId == projectId && up.UserId == userId);

        if (userProject == null) return false;

        userProject.IsActive = false;
        userProject.RemovedBy = removedBy;
        userProject.RemovedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignManagerToProjectAsync(Guid projectId, Guid managerId, Guid assignedBy)
    {
        var existing = await _context.ProjectManagers
            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.ManagerId == managerId);

        if (existing != null) return true;

        var projectManager = new ProjectManager
        {
            ProjectId = projectId,
            ManagerId = managerId,
            AssignedBy = assignedBy
        };

        _context.ProjectManagers.Add(projectManager);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveManagerFromProjectAsync(Guid projectId, Guid managerId)
    {
        var projectManager = await _context.ProjectManagers
            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.ManagerId == managerId);

        if (projectManager == null) return false;

        _context.ProjectManagers.Remove(projectManager);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<UserDto>> GetProjectMembersAsync(Guid projectId)
    {
        var users = await _context.UserProjects
            .Where(up => up.ProjectId == projectId && up.IsActive)
            .Include(up => up.User)
            .Select(up => up.User!)
            .ToListAsync();

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            FullName = u.FullName,
            AvatarUrl = u.AvatarUrl,
            Role = u.Role.ToString(),
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        }).ToList();
    }

    private static ProjectDto MapToDto(Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Client = project.Client,
            Status = project.Status.ToString(),
            HourlyRate = project.HourlyRate,
            CreatedBy = project.CreatedBy,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt,
            Creator = project.Creator != null ? new UserDto
            {
                Id = project.Creator.Id,
                Email = project.Creator.Email,
                FullName = project.Creator.FullName,
                Role = project.Creator.Role.ToString(),
                IsActive = project.Creator.IsActive,
                CreatedAt = project.Creator.CreatedAt,
                UpdatedAt = project.Creator.UpdatedAt
            } : null
        };
    }
}
