using AISmartSheet.API.DTOs;
using AISmartSheet.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AISmartSheet.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly SupabaseAuthService _supabaseAuthService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(SupabaseAuthService supabaseAuthService, ILogger<AuthController> logger)
    {
        _supabaseAuthService = supabaseAuthService;
        _logger = logger;
    }

    /// <summary>
    /// User login endpoint
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Invalid request data",
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
            });
        }

        var result = await _supabaseAuthService.LoginAsync(request);

        if (result == null)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "Invalid email or password",
                Errors = new List<string> { "Authentication failed" }
            });
        }

        return Ok(new ApiResponse<LoginResponse>
        {
            Success = true,
            Message = "Login successful",
            Data = result
        });
    }

    /// <summary>
    /// User registration endpoint
    /// </summary>
    /// <param name="request">Registration information</param>
    /// <returns>Created user information</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Invalid request data",
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
            });
        }

        var result = await _supabaseAuthService.RegisterAsync(request);

        if (result == null)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Registration failed",
                Errors = new List<string> { "User with this email already exists or registration failed" }
            });
        }

        return CreatedAtAction(nameof(Register), new ApiResponse<LoginResponse>
        {
            Success = true,
            Message = "User registered successfully",
            Data = result
        });
    }

    /// <summary>
    /// Verify token validity
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("verify")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public IActionResult Verify()
    {
        var userEmail = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
        var userName = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;
        var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Token is valid",
            Data = new
            {
                email = userEmail,
                name = userName,
                role = userRole,
                authenticated = true
            }
        });
    }
}
