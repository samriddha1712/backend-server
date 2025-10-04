using AISmartSheet.API.Services;
using System.Security.Claims;

namespace AISmartSheet.API.Middleware;

public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;

    public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAuditLogService auditLogService)
    {
        // Only log non-GET requests that require authentication
        if (context.Request.Method != "GET" && context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (Guid.TryParse(userId, out var userGuid))
            {
                var action = $"{context.Request.Method} {context.Request.Path}";
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();

                try
                {
                    await auditLogService.LogActionAsync(
                        userGuid,
                        action,
                        "HTTP_REQUEST",
                        null,
                        $"Request to {context.Request.Path}",
                        ipAddress
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to log audit entry for action: {Action}", action);
                }
            }
        }

        await _next(context);
    }
}
