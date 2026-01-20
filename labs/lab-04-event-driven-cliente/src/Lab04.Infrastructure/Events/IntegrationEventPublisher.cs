using System.Text;
using System.Text.Json;
using Lab04.Core.Contract.Events;
using Lab04.Core.Events.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Lab04.Infrastructure.Events;

/// <summary>
/// Implementação do publisher de eventos de integração usando RabbitMQ
/// </summary>
public class IntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly IConnection _connection;
    private readonly ILogger<IntegrationEventPublisher> _logger;
    private readonly string _exchangeName;

    public IntegrationEventPublisher(
        IConnection connection,
        IConfiguration configuration,
        ILogger<IntegrationEventPublisher> logger)
    {
        _connection = connection;
        _logger = logger;
        _exchangeName = configuration["RabbitMQ:Exchange"] ?? "lab04.exchange";
    }

    public async Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        await PublishAsync(integrationEvent, integrationEvent.EventType, cancellationToken);
    }

    public Task PublishAsync<TEvent>(TEvent integrationEvent, string routingKey, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        _logger.LogInformation("Publishing integration event: {EventType} with routing key: {RoutingKey}",
            integrationEvent.EventType, routingKey);

        using var channel = _connection.CreateModel();
        
        // Declarar o exchange
        channel.ExchangeDeclare(
            exchange: _exchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        var message = JsonSerializer.Serialize(integrationEvent);
        var body = Encoding.UTF8.GetBytes(message);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.MessageId = integrationEvent.EventId.ToString();
        properties.CorrelationId = integrationEvent.CorrelationId;
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        properties.Type = integrationEvent.EventType;

        channel.BasicPublish(
            exchange: _exchangeName,
            routingKey: routingKey,
            basicProperties: properties,
            body: body);

        _logger.LogInformation("Integration event published successfully: {EventType} - EventId: {EventId}",
            integrationEvent.EventType, integrationEvent.EventId);

        return Task.CompletedTask;
    }
}
