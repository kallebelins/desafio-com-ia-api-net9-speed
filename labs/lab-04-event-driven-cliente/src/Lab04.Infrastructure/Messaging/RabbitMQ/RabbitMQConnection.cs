using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Lab04.Infrastructure.Messaging.RabbitMQ;

/// <summary>
/// Factory para criar conexões RabbitMQ
/// </summary>
public static class RabbitMQConnectionFactory
{
    public static IConnection CreateConnection(IConfiguration configuration, ILogger? logger = null)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
            Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = configuration["RabbitMQ:UserName"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest",
            VirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/",
            DispatchConsumersAsync = true
        };

        logger?.LogInformation("Connecting to RabbitMQ at {Host}:{Port}", factory.HostName, factory.Port);

        var connection = factory.CreateConnection();

        logger?.LogInformation("Connected to RabbitMQ successfully");

        return connection;
    }
}
