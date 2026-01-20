using Microsoft.Extensions.DependencyInjection;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab05.Application.Infrastructure;

/// <summary>
/// Implementação simples do IMediator que resolve handlers via DI
/// </summary>
public class SimpleMediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public SimpleMediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> SendAsync<TResponse>(IMediatorCommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IMediatorCommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)command, cancellationToken);
    }

    public async Task<TResponse> SendAsync<TResponse>(IMediatorQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IMediatorQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)query, cancellationToken);
    }

    public Task SendAsync(IMediatorCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Command without response not implemented");
    }

    public Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default) 
        where TNotification : IMediatorNotification
    {
        throw new NotImplementedException("Publish not implemented");
    }

    public Task<TResponse> SendAsync<TResponse>(IMediatorRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        // Try to determine if it's a command or query based on interface
        if (request is IMediatorCommand<TResponse> command)
        {
            return SendAsync(command, cancellationToken);
        }
        
        if (request is IMediatorQuery<TResponse> query)
        {
            return SendAsync(query, cancellationToken);
        }

        throw new InvalidOperationException($"Request type {request.GetType().Name} is not supported");
    }

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Streaming not implemented");
    }
}
