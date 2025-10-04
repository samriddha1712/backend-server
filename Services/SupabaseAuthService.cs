using Supabase;
using AISmartSheet.API.DTOs;
using AISmartSheet.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AISmartSheet.API.Services
{
    public class SupabaseAuthService
    {
        private readonly Supabase.Client _supabase;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SupabaseAuthService> _logger;

        public SupabaseAuthService(
            Supabase.Client supabase,
            IConfiguration configuration,
            ILogger<SupabaseAuthService> logger)
        {
            _supabase = supabase;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            try
            {
                _logger.LogInformation("🔐 Attempting database login for: {Email}", request.Email);

                // Query Supabase database directly for user using REST API
                var response = await _supabase
                    .From<SupabaseUser>()
                    .Filter("email", Postgrest.Constants.Operator.ILike, request.Email)
                    .Single();

                if (response == null)
                {
                    _logger.LogWarning("Login failed - user not found: {Email}", request.Email);
                    return null;
                }

                var user = response;

                // Check if user is active
                if (!user.IsActive)
                {
                    _logger.LogWarning("Login failed - user is inactive: {Email}", request.Email);
                    return null;
                }

                // Verify password using BCrypt
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Login failed - invalid password: {Email}", request.Email);
                    return null;
                }

                _logger.LogInformation("✅ Database login successful for: {Email}", request.Email);

                // Create user DTO from database user
                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role.ToLower(),
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                // Generate custom JWT token
                var token = GenerateCustomJwtToken(userDto);
                
                // Calculate expiration
                var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "480");
                var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

                return new LoginResponse
                {
                    Token = token,
                    User = userDto,
                    ExpiresAt = expiresAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during database login for: {Email}", request.Email);
                return null;
            }
        }

        public async Task<LoginResponse?> RegisterAsync(RegisterRequest request)
        {
            try
            {
                _logger.LogInformation("📝 Attempting database registration for: {Email}", request.Email);

                // Check if user already exists
                var existingUserCheck = await _supabase
                    .From<SupabaseUser>()
                    .Filter("email", Postgrest.Constants.Operator.ILike, request.Email)
                    .Get();

                if (existingUserCheck.Models.Any())
                {
                    _logger.LogWarning("Registration failed - user already exists: {Email}", request.Email);
                    return null;
                }

                // Hash password with BCrypt
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create new user
                var newUser = new SupabaseUser
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email.ToLower(),
                    FullName = request.FullName,
                    PasswordHash = passwordHash,
                    Role = request.Role.ToLower(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Save to database using Supabase
                var insertResponse = await _supabase
                    .From<SupabaseUser>()
                    .Insert(newUser);

                var createdUser = insertResponse.Models.FirstOrDefault();
                if (createdUser == null)
                {
                    _logger.LogError("Failed to create user in database");
                    return null;
                }

                _logger.LogInformation("✅ Database registration successful for: {Email}", request.Email);

                // Create user DTO
                var userDto = new UserDto
                {
                    Id = createdUser.Id,
                    Email = createdUser.Email,
                    FullName = createdUser.FullName,
                    Role = createdUser.Role.ToLower(),
                    IsActive = createdUser.IsActive,
                    CreatedAt = createdUser.CreatedAt,
                    UpdatedAt = createdUser.UpdatedAt
                };

                // Generate custom JWT token
                var token = GenerateCustomJwtToken(userDto);
                
                // Calculate expiration
                var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "480");
                var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

                return new LoginResponse
                {
                    Token = token,
                    User = userDto,
                    ExpiresAt = expiresAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during database registration for: {Email}", request.Email);
                return null;
            }
        }

        public Task<bool> VerifyTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? "");

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        private string GenerateCustomJwtToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? "");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "480")),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
