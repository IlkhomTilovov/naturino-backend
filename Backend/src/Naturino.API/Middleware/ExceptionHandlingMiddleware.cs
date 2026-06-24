using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Naturino.Domain.Exceptions;

namespace Naturino.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            UnauthorizedException => (HttpStatusCode.Unauthorized, "Unauthorized"),
            Domain.Exceptions.ValidationException => (HttpStatusCode.BadRequest, "Validation failed"),
            ConflictException => (HttpStatusCode.Conflict, "Conflict"),
            ForbiddenException => (HttpStatusCode.Forbidden, "Forbidden"),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception for request {Method} {Path}", context.Request.Method, context.Request.Path);
        }
        else
        {
            _logger.LogWarning("{ExceptionType} for request {Method} {Path}: {Message}", exception.GetType().Name, context.Request.Method, context.Request.Path, exception.Message);
        }

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = context.Request.Path,
            Extensions =
            {
                ["traceId"] = context.TraceIdentifier
            }
        };

        if (exception is Domain.Exceptions.ValidationException validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors;
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}
