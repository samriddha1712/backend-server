using AISmartSheet.API.DTOs;
using AISmartSheet.API.Models;

namespace AISmartSheet.API.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<UserDto?> RegisterAsync(RegisterRequest request);
    string GenerateJwtToken(User user);
}
