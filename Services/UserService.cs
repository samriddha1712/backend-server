using AISmartSheet.API.Data;
using AISmartSheet.API.DTOs;
using AISmartSheet.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AISmartSheet.API.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResponse<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 20, string? search = null, string? role = null)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.Email.Contains(search) || u.FullName.Contains(search));
        }

        if (!string.IsNullOrEmpty(role) && Enum.TryParse<UserRole>(role, true, out var userRole))
        {
            query = query.Where(u => u.Role == userRole);
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var users = await query
            .OrderBy(u => u.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<UserDto>
        {
            Data = users.Select(MapToDto).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto?> CreateUserAsync(CreateUserRequest request, Guid createdBy)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

        if (existingUser != null) return null;

        if (!Enum.TryParse<UserRole>(request.Role, true, out var userRole))
        {
            userRole = UserRole.developer;
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Email = request.Email.ToLower(),
            FullName = request.FullName,
            PasswordHash = passwordHash,
            Role = userRole,
            AddedBy = createdBy,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<UserDto?> UpdateUserAsync(Guid userId, UpdateUserRequest request, Guid updatedBy)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        if (!string.IsNullOrEmpty(request.FullName))
            user.FullName = request.FullName;

        if (request.AvatarUrl != null)
            user.AvatarUrl = request.AvatarUrl;

        if (!string.IsNullOrEmpty(request.Role) && Enum.TryParse<UserRole>(request.Role, true, out var userRole))
            user.Role = userRole;

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(user);
    }

    public async Task<bool> DeleteUserAsync(Guid userId, Guid deletedBy)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeactivateUserAsync(Guid userId, Guid deactivatedBy)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.IsActive = false;
        user.DeactivatedBy = deactivatedBy;
        user.DeactivatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ActivateUserAsync(Guid userId, Guid activatedBy)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.IsActive = true;
        user.DeactivatedBy = null;
        user.DeactivatedAt = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
