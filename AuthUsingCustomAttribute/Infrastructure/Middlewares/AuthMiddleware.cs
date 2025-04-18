using System.IdentityModel.Tokens.Jwt;
using AuthUsingCustomAttribute.Application.Attributes;
using AuthUsingCustomAttribute.Domain.Enums;

namespace AuthUsingCustomAttribute.Infrastructure.Middlewares;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthMiddleware> _logger;

    public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("AuthorizationMiddleware invoked");
        var endpoint = context.GetEndpoint();
        var authAttr = endpoint?.Metadata.GetMetadata<AuthorizeRolesAttribute>();
        
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (string.IsNullOrEmpty(token))
        {
            if (authAttr != null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Missing token for protected endpoint.");
                return;
            }
            await _next(context);
            return;
        }

        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            
            var userId = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var roleId = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleId))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid token: missing claims.");
                return;
            }

            if (authAttr != null && !authAttr.Roles.Contains((UserRoleEnum)int.Parse(roleId)))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Access denied: role not authorized.");
                return;
            }

            await _next(context);
            
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("JWT error: " + ex.Message);
        }
    }
}