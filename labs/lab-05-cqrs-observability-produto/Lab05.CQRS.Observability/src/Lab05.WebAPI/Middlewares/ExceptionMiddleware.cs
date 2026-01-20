using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Lab05.WebAPI.Middlewares;

/// <summary>
/// Middleware global para tratamento de exceções
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred. Path: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                "Erro de validação",
                validationEx.Errors.Select(e => e.ErrorMessage).ToList()
            ),
            ArgumentException => (
                HttpStatusCode.BadRequest,
                exception.Message,
                (List<string>?)null
            ),
            KeyNotFoundException => (
                HttpStatusCode.NotFound,
                exception.Message,
                (List<string>?)null
            ),
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                "Não autorizado",
                (List<string>?)null
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "Ocorreu um erro ao processar sua requisição",
                (List<string>?)null
            )
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            StatusCode = (int)statusCode,
            Message = message,
            Errors = errors,
            Timestamp = DateTime.UtcNow,
            TraceId = context.TraceIdentifier
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}

/// <summary>
/// Extension methods para o ExceptionMiddleware
/// </summary>
public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}
