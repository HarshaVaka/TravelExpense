using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace UserService.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        // ensure a correlation id is present
        var correlationId = httpContext.Request.Headers.ContainsKey("X-Correlation-ID")
            ? httpContext.Request.Headers["X-Correlation-ID"].ToString()
            : Guid.NewGuid().ToString();

        // add to response for visibility
        httpContext.Response.Headers["X-Correlation-ID"] = correlationId;

        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception (CorrelationId={CorrelationId})", correlationId);
            await HandleExceptionAsync(httpContext, ex, correlationId);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, string correlationId)
    {
        context.Response.ContentType = "application/json";

        int statusCode = (int)HttpStatusCode.InternalServerError;
        object? responseObj = null;

        switch (exception)
        {
            case ValidationException fv:
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                var errors = fv.Errors?.Select(e => new { field = e.PropertyName, error = e.ErrorMessage });
                responseObj = new { message = "Validation failed", errors, correlationId };
                break;
            }
            case DbUpdateException dbEx:
            {
                // check for SQL Server unique constraint error numbers (2601, 2627)
                var sqlEx = dbEx.InnerException as SqlException;
                if (sqlEx != null && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
                {
                    statusCode = (int)HttpStatusCode.Conflict;
                    responseObj = new { message = "A database conflict occurred (unique constraint violation).", correlationId };
                }
                else
                {
                    // fall back to text inspection
                    var inner = dbEx.InnerException?.Message ?? string.Empty;
                    if (!string.IsNullOrEmpty(inner) && (inner.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) || inner.Contains("duplicate", StringComparison.OrdinalIgnoreCase) || inner.Contains("IX_", StringComparison.OrdinalIgnoreCase)))
                    {
                        statusCode = (int)HttpStatusCode.Conflict;
                        responseObj = new { message = "A database conflict occurred (possible unique constraint violation).", correlationId };
                    }
                    else
                    {
                        statusCode = (int)HttpStatusCode.InternalServerError;
                        responseObj = new { message = "A database error occurred.", correlationId };
                    }
                }

                break;
            }
            default:
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                responseObj = new { message = "An unexpected error occurred.", correlationId };
                break;
            }
        }

        context.Response.StatusCode = statusCode;
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var payload = JsonSerializer.Serialize(responseObj, options);
        return context.Response.WriteAsync(payload ?? string.Empty);
    }
}
