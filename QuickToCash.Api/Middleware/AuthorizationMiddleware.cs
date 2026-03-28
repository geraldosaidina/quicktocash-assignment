using System.Text.Json;
using QuickToCash.Api.Common;

namespace QuickToCash.Api.Middleware;

public class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader) ||
            string.IsNullOrWhiteSpace(authorizationHeader.ToString()))
        {
            await WriteUnauthorized(context, "Authorization header is required.");
            return;
        }

        var token = authorizationHeader.ToString();
        if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            await WriteUnauthorized(context, "Authorization header must use Bearer scheme.");
            return;
        }

        await _next(context);
    }

    private static async Task WriteUnauthorized(HttpContext context, string error)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var payload = ApiResponse<object>.Fail("Unauthorized.", new[] { error });
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
