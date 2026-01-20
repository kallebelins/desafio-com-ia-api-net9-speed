using Lab09.Application.Commands;
using Lab09.Application.DTOs;
using Lab09.Application.Handlers.Commands;
using Lab09.Application.Handlers.Queries;
using Lab09.Application.Projections;
using Lab09.Application.Queries;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab09.WebAPI.Extensions;

/// <summary>
/// Extensões para configuração de serviços
/// </summary>
public static class ServiceBuilderExtensions
{
    /// <summary>
    /// Adiciona os serviços do Mvp24Hours CQRS
    /// </summary>
    public static IServiceCollection AddMvp24HoursCqrs(this IServiceCollection services)
    {
        // Command Handlers
        services.AddScoped<IMediatorCommandHandler<IniciarVendaCommand, VendaDto>, VendaCommandHandler>();
        services.AddScoped<IMediatorCommandHandler<AdicionarItemCommand, VendaDto>, VendaCommandHandler>();
        services.AddScoped<IMediatorCommandHandler<RemoverItemCommand, VendaDto>, VendaCommandHandler>();
        services.AddScoped<IMediatorCommandHandler<AplicarDescontoCommand, VendaDto>, VendaCommandHandler>();
        services.AddScoped<IMediatorCommandHandler<FinalizarVendaCommand, VendaDto>, VendaCommandHandler>();
        services.AddScoped<IMediatorCommandHandler<CancelarVendaCommand, VendaDto>, VendaCommandHandler>();

        // Query Handlers
        services.AddScoped<IMediatorQueryHandler<GetVendaByIdQuery, VendaDto?>, VendaQueryHandler>();
        services.AddScoped<IMediatorQueryHandler<GetVendaHistoryQuery, VendaHistoryDto?>, VendaQueryHandler>();
        services.AddScoped<IMediatorQueryHandler<GetVendaAtMomentQuery, VendaDto?>, VendaQueryHandler>();
        services.AddScoped<IMediatorQueryHandler<GetVendasPorPeriodoQuery, IEnumerable<VendaReadModel>>, VendaQueryHandler>();

        return services;
    }
}
