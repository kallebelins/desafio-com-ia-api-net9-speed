using Lab04.Application.EventHandlers.Domain;
using Lab04.Application.EventHandlers.Integration;
using Lab04.Application.Services;
using Lab04.Core.Contract.Events;
using Lab04.Core.Contract.Services;
using Lab04.Core.Events.Domain;
using Lab04.Core.Events.Integration;
using Lab04.Infrastructure.Data;
using Lab04.Infrastructure.Events;
using Lab04.Infrastructure.Messaging.RabbitMQ;
using Lab04.WebAPI.HostedServices;
using Microsoft.EntityFrameworkCore;
using Mvp24Hours.Extensions;
using RabbitMQ.Client;

namespace Lab04.WebAPI.Extensions;

/// <summary>
/// Extension methods para registro de serviços
/// </summary>
public static class ServiceBuilderExtensions
{
    public static IServiceCollection AddMyServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ===============================================
        // Database (EF Core + SQL Server)
        // ===============================================
        // NÃO usar EnableRetryOnFailure - conflita com UoW do Mvp24Hours
        // Ref: learnings/lab02-learning.md
        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddMvp24HoursDbContext<DataContext>();
        services.AddMvp24HoursRepositoryAsync(options =>
        {
            options.MaxQtyByQueryPage = 100;
        });

        // ===============================================
        // RabbitMQ Connection
        // ===============================================
        services.AddSingleton<IConnection>(sp =>
        {
            var logger = sp.GetService<ILogger<IConnection>>();
            return RabbitMQConnectionFactory.CreateConnection(configuration, logger);
        });

        // ===============================================
        // Event Infrastructure
        // ===============================================
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();

        // ===============================================
        // Domain Event Handlers
        // ===============================================
        services.AddScoped<IDomainEventHandler<ClienteCriadoEvent>, ClienteCriadoEventHandler>();
        // Adicione outros handlers aqui conforme necessário:
        // services.AddScoped<IDomainEventHandler<ClienteAtualizadoEvent>, ClienteAtualizadoEventHandler>();
        // services.AddScoped<IDomainEventHandler<ClienteExcluidoEvent>, ClienteExcluidoEventHandler>();

        // ===============================================
        // Integration Event Handlers
        // ===============================================
        services.AddScoped<IIntegrationEventHandler<ClienteCriadoIntegrationEvent>, EmailBoasVindasHandler>();

        // ===============================================
        // Services
        // ===============================================
        services.AddScoped<IClienteService, ClienteService>();

        // ===============================================
        // Hosted Services (Background Workers)
        // ===============================================
        services.AddHostedService<IntegrationEventConsumerService>();

        return services;
    }
}
