using System.Diagnostics;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using OpenTelemetry.Trace;

namespace Lab10.Application.Behaviors;

/// <summary>
/// Behavior de tracing para OpenTelemetry
/// </summary>
public class TracingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMediatorRequest<TResponse>
{
    private static readonly ActivitySource ActivitySource = new("Lab10.CQRS");

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestType = request is IMediatorCommand<TResponse> ? "Command" : "Query";

        using var activity = ActivitySource.StartActivity($"CQRS.{requestName}");
        activity?.SetTag("cqrs.request_type", requestType);
        activity?.SetTag("cqrs.request_name", requestName);

        try
        {
            var response = await next();
            activity?.SetStatus(ActivityStatusCode.Ok);
            return response;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddTag("exception.type", ex.GetType().FullName);
            activity?.AddTag("exception.message", ex.Message);
            activity?.AddTag("exception.stacktrace", ex.StackTrace);
            throw;
        }
    }
}
