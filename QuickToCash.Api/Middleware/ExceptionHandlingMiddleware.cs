using System.Text.Json;
using QuickToCash.Api.Common;

namespace QuickToCash.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = ApiResponse<object>.Fail(
                "An unexpected error occurred.",
                new[] { "Please contact support if the issue persists." });

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
