using AISmartSheet.API.DTOs;
using AISmartSheet.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AISmartSheet.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users with pagination and filtering
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? role = null)
    {
        var result = await _userService.GetAllUsersAsync(page, pageSize, search, role);

        return Ok(new ApiResponse<PagedResponse<UserDto>>
        {
            Success = true,
            Data = result
        });
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "User not found"
            });
        }

        return Ok(new ApiResponse<UserDto>
        {
            Success = true,
            Data = user
        });
    }

    /// <summary>
    /// Create new user
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _userService.CreateUserAsync(request, currentUserId);

        if (user == null)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "User with this email already exists"
            });
        }

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, new ApiResponse<UserDto>
        {
            Success = true,
            Message = "User created successfully",
            Data = user
        });
    }

    /// <summary>
    /// Update user
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _userService.UpdateUserAsync(id, request, currentUserId);

        if (user == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "User not found"
            });
        }

        return Ok(new ApiResponse<UserDto>
        {
            Success = true,
            Message = "User updated successfully",
            Data = user
        });
    }

    /// <summary>
    /// Delete user
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var success = await _userService.DeleteUserAsync(id, currentUserId);

        if (!success)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "User not found"
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "User deleted successfully"
        });
    }

    /// <summary>
    /// Deactivate user
    /// </summary>
    [HttpPost("{id}/deactivate")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var success = await _userService.DeactivateUserAsync(id, currentUserId);

        if (!success)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "User not found"
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "User deactivated successfully"
        });
    }

    /// <summary>
    /// Activate user
    /// </summary>
    [HttpPost("{id}/activate")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ActivateUser(Guid id)
    {
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var success = await _userService.ActivateUserAsync(id, currentUserId);

        if (!success)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "User not found"
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "User activated successfully"
        });
    }
}
