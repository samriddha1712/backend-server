using AISmartSheet.API.Data;
using AISmartSheet.API.DTOs;
using AISmartSheet.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace AISmartSheet.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            User? user = null;
            
            try
            {
                user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
            }
            catch (Exception dbEx)
            {
                _logger.LogWarning(dbEx, "Database not available, using mock authentication for development");
                
                // Mock user for development/testing when database is not available
                // TODO: Remove this in production - Accepts ANY email/password for development
                _logger.LogInformation("🔧 Creating mock user for: {Email} - Database not connected", request.Email);
                
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email,
                    FullName = request.Email.Split('@')[0], // Use email prefix as name
                    Role = UserRole.developer,
                    IsActive = true,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }

            if (user == null)
            {
                _logger.LogWarning("Login attempt for non-existent user: {Email}", request.Email);
                return null;
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login attempt for deactivated user: {Email}", request.Email);
                return null;
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for user: {Email}", request.Email);
                return null;
            }

            var token = GenerateJwtToken(user);
            var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationMinutes", 480);

            _logger.LogInformation("User logged in successfully: {Email}", request.Email);

            return new LoginResponse
            {
                Token = token,
                User = MapToUserDto(user),
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {Email}", request.Email);
            throw;
        }
    }

    public async Task<UserDto?> RegisterAsync(RegisterRequest request)
    {
        try
        {
            User? existingUser = null;
            bool isDatabaseAvailable = true;
            
            try
            {
                // Check if user already exists
                existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
            }
            catch (Exception dbEx)
            {
                _logger.LogWarning(dbEx, "Database not available during registration check");
                isDatabaseAvailable = false;
            }

            if (existingUser != null)
            {
                _logger.LogWarning("Registration attempt with existing email: {Email}", request.Email);
                return null;
            }

            // Validate role
            if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var userRole))
            {
                userRole = UserRole.developer;
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User
            {
                Email = request.Email.ToLower(),
                FullName = request.FullName,
                PasswordHash = passwordHash,
                Role = userRole,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (isDatabaseAvailable)
            {
                try
                {
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("New user registered: {Email}", request.Email);
                }
                catch (Exception dbEx)
                {
                    _logger.LogWarning(dbEx, "Could not save to database, returning mock user");
                    _logger.LogInformation("🔧 Mock registration successful (Database not connected): {Email}", request.Email);
                }
            }
            else
            {
                _logger.LogInformation("🔧 Mock registration successful (Database not connected): {Email}", request.Email);
            }

            return MapToUserDto(newUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user: {Email}", request.Email);
            throw;
        }
    }

    public string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expirationMinutes = jwtSettings.GetValue<int>("ExpirationMinutes", 480);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("is_active", user.IsActive.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static UserDto MapToUserDto(User user)
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
