using System.Net;
using System.Text.Json;
using OrderManagement.Application.Common.Exceptions;

namespace OrderManagement.Api.Middleware;

// Centralized error handling: controllers stay free of try/catch.
// Domain InvalidOperationException / ArgumentException and
// application BusinessRuleException become 400;
// NotFoundException becomes 404; everything else 500.
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var (status, title) = ex switch
            {
                NotFoundException => (HttpStatusCode.NotFound, "Not found"),
                BusinessRuleException => (HttpStatusCode.BadRequest, "Business rule violated"),
                InvalidOperationException => (HttpStatusCode.BadRequest, "Business rule violated"),
                ArgumentException => (HttpStatusCode.BadRequest, "Invalid input"),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred"),
            };

            if (status == HttpStatusCode.InternalServerError)
                logger.LogError(ex, "Unhandled exception");
            else
                logger.LogWarning(ex, "Request failed with {Status}", (int)status);

            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/json";

            var body = JsonSerializer.Serialize(new
            {
                title,
                detail = ex.Message,
                status = (int)status
            });

            await context.Response.WriteAsync(body);
        }
    }
}
