using System.Text;
using System.Text.Json;
using Lab04.Core.Contract.Events;
using Lab04.Core.Events.Integration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lab04.WebAPI.HostedServices;

/// <summary>
/// Hosted Service responsável por consumir eventos de integração do RabbitMQ
/// </summary>
public class IntegrationEventConsumerService : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IntegrationEventConsumerService> _logger;
    private readonly string _exchangeName;
    private readonly string _queueName;
    private IModel? _channel;

    public IntegrationEventConsumerService(
        IConnection connection,
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<IntegrationEventConsumerService> logger)
    {
        _connection = connection;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _exchangeName = configuration["RabbitMQ:Exchange"] ?? "lab04.exchange";
        _queueName = "lab04.cliente.boas-vindas.queue";
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Integration Event Consumer Service...");

        _channel = _connection.CreateModel();

        // Declarar exchange
        _channel.ExchangeDeclare(
            exchange: _exchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        // Declarar fila
        _channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        // Bind da fila ao exchange com routing key para eventos de cliente
        _channel.QueueBind(
            queue: _queueName,
            exchange: _exchangeName,
            routingKey: "cliente.*");

        _logger.LogInformation(
            "Listening for messages on queue '{QueueName}' with routing key 'cliente.*'",
            _queueName);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var eventType = ea.BasicProperties.Type;

            _logger.LogInformation(
                "Received message: EventType={EventType}, RoutingKey={RoutingKey}",
                eventType, ea.RoutingKey);

            try
            {
                await ProcessEventAsync(eventType, message, stoppingToken);
                _channel.BasicAck(ea.DeliveryTag, false);
                _logger.LogInformation("Message processed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing event: {EventType}", eventType);
                // Requeue a mensagem em caso de erro
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(
            queue: _queueName,
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("Integration Event Consumer Service started successfully");

        return Task.CompletedTask;
    }

    private async Task ProcessEventAsync(string? eventType, string message, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        switch (eventType)
        {
            case nameof(ClienteCriadoIntegrationEvent):
                var clienteEvent = JsonSerializer.Deserialize<ClienteCriadoIntegrationEvent>(message);
                if (clienteEvent != null)
                {
                    var handler = scope.ServiceProvider
                        .GetRequiredService<IIntegrationEventHandler<ClienteCriadoIntegrationEvent>>();
                    await handler.HandleAsync(clienteEvent, cancellationToken);
                }
                break;

            default:
                _logger.LogWarning("Unknown event type: {EventType}", eventType);
                break;
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        base.Dispose();
    }
}
