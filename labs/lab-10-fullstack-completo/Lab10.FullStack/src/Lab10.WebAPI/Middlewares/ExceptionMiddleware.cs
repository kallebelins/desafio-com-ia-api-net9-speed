using System.Net;
using System.Text.Json;
using FluentValidation;
using Lab10.Domain.Exceptions;

namespace Lab10.WebAPI.Middlewares;

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
            _logger.LogError(ex, "Exceção não tratada");
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
                validationEx.Errors.Select(e => e.ErrorMessage).ToList()),
            DomainException domainEx => (
                HttpStatusCode.BadRequest,
                domainEx.Message,
                new List<string>()),
            ArgumentException argEx => (
                HttpStatusCode.BadRequest,
                argEx.Message,
                new List<string>()),
            KeyNotFoundException => (
                HttpStatusCode.NotFound,
                "Recurso não encontrado",
                new List<string>()),
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                "Não autorizado",
                new List<string>()),
            _ => (
                HttpStatusCode.InternalServerError,
                "Erro interno do servidor",
                new List<string>())
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            StatusCode = (int)statusCode,
            Message = message,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
