using AISmartSheet.API.DTOs;
using AISmartSheet.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AISmartSheet.API.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }

    /// <summary>
    /// Get all projects with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<ProjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllProjects(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null)
    {
        var result = await _projectService.GetAllProjectsAsync(page, pageSize, search, status);

        return Ok(new ApiResponse<PagedResponse<ProjectDto>>
        {
            Success = true,
            Data = result
        });
    }

    /// <summary>
    /// Get user's assigned projects
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<List<ProjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserProjects(Guid userId)
    {
        var projects = await _projectService.GetUserProjectsAsync(userId);

        return Ok(new ApiResponse<List<ProjectDto>>
        {
            Success = true,
            Data = projects
        });
    }

    /// <summary>
    /// Get manager's managed projects
    /// </summary>
    [HttpGet("manager/{managerId}")]
    [Authorize(Policy = "ManagerOrAbove")]
    [ProducesResponseType(typeof(ApiResponse<List<ProjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetManagerProjects(Guid managerId)
    {
        var projects = await _projectService.GetManagerProjectsAsync(managerId);

        return Ok(new ApiResponse<List<ProjectDto>>
        {
            Success = true,
            Data = projects
        });
    }

    /// <summary>
    /// Get project by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectById(Guid id)
    {
        var project = await _projectService.GetProjectByIdAsync(id);

        if (project == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Project not found"
            });
        }

        return Ok(new ApiResponse<ProjectDto>
        {
            Success = true,
            Data = project
        });
    }

    /// <summary>
    /// Create new project
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
    {
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var project = await _projectService.CreateProjectAsync(request, currentUserId);

        return CreatedAtAction(nameof(GetProjectById), new { id = project!.Id }, new ApiResponse<ProjectDto>
        {
            Success = true,
            Message = "Project created successfully",
            Data = project
        });
    }

    /// <summary>
    /// Update project
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectRequest request)
    {
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var project = await _projectService.UpdateProjectAsync(id, request, currentUserId);

        if (project == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Project not found"
            });
        }

        return Ok(new ApiResponse<ProjectDto>
        {
            Success = true,
            Message = "Project updated successfully",
            Data = project
        });
    }

    /// <summary>
    /// Delete project
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var success = await _projectService.DeleteProjectAsync(id);

        if (!success)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Project not found"
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Project deleted successfully"
        });
    }

    /// <summary>
    /// Assign user to project
    /// </summary>
    [HttpPost("{id}/assign-user")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignUserToProject(Guid id, [FromBody] AssignUserToProjectRequest request)
    {
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var success = await _projectService.AssignUserToProjectAsync(id, request.UserId, currentUserId);

        return Ok(new ApiResponse<object>
        {
            Success = success,
            Message = success ? "User assigned to project successfully" : "Failed to assign user"
        });
    }

    /// <summary>
    /// Remove user from project
    /// </summary>
    [HttpPost("{id}/remove-user")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveUserFromProject(Guid id, [FromBody] AssignUserToProjectRequest request)
    {
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var success = await _projectService.RemoveUserFromProjectAsync(id, request.UserId, currentUserId);

        return Ok(new ApiResponse<object>
        {
            Success = success,
            Message = success ? "User removed from project successfully" : "Failed to remove user"
        });
    }

    /// <summary>
    /// Get project members
    /// </summary>
    [HttpGet("{id}/members")]
    [Authorize(Policy = "ManagerOrAbove")]
    [ProducesResponseType(typeof(ApiResponse<List<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectMembers(Guid id)
    {
        var members = await _projectService.GetProjectMembersAsync(id);

        return Ok(new ApiResponse<List<UserDto>>
        {
            Success = true,
            Data = members
        });
    }
}
