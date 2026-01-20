using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Lab10.Application.Behaviors;
using Lab10.Application.Commands.Categorias;
using Lab10.Application.Commands.Clientes;
using Lab10.Application.Commands.Produtos;
using Lab10.Application.Commands.Vendas;
using Lab10.Application.DTOs;
using Lab10.Application.Handlers.Commands;
using Lab10.Application.Handlers.Queries;
using Lab10.Application.Infrastructure;
using Lab10.Application.Interfaces;
using Lab10.Application.Queries.Categorias;
using Lab10.Application.Queries.Clientes;
using Lab10.Application.Queries.Produtos;
using Lab10.Application.Queries.Vendas;
using Lab10.Application.Sagas;
using Lab10.Application.Sagas.Steps;
using Lab10.Application.Validators;
using Lab10.Domain.Interfaces;
using Lab10.Domain.Services;
using Lab10.Infrastructure.Data;
using Lab10.Infrastructure.Data.Repositories;
using Lab10.Infrastructure.Email;
using Lab10.Infrastructure.ExternalServices;
using Lab10.Infrastructure.Outbox;

namespace Lab10.WebAPI.Extensions;

public static class ServiceBuilderExtensions
{
    public static IServiceCollection AddMyServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<WriteDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("WriteDatabase")));

        // Unit of Work
        services.AddScoped<IUnitOfWorkApplication, UnitOfWork>();

        // Repositories
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IVendaRepository, VendaRepository>();
        services.AddScoped<IPagamentoRepository, PagamentoRepository>();

        // Domain Services
        services.AddScoped<VendaDomainService>();

        // Application Services
        services.AddScoped<IOutboxService, OutboxService>();
        services.AddScoped<IPagamentoGateway, PagamentoGateway>();
        services.AddScoped<IEmailService, SmtpEmailService>();

        // CQRS Mediator
        services.AddScoped<IMediator, SimpleMediator>();

        // Command Handlers
        services.AddScoped<IMediatorCommandHandler<CreateClienteCommand, IBusinessResult<ClienteDto>>, CreateClienteCommandHandler>();
        services.AddScoped<IMediatorCommandHandler<UpdateClienteCommand, IBusinessResult<ClienteDto>>, UpdateClienteCommandHandler>();
        services.AddScoped<IMediatorCommandHandler<CreateCategoriaCommand, IBusinessResult<CategoriaDto>>, CreateCategoriaCommandHandler>();
        services.AddScoped<IMediatorCommandHandler<CreateProdutoCommand, IBusinessResult<ProdutoDto>>, CreateProdutoCommandHandler>();
        services.AddScoped<IMediatorCommandHandler<AtualizarEstoqueCommand, IBusinessResult<ProdutoDto>>, AtualizarEstoqueCommandHandler>();
        services.AddScoped<IMediatorCommandHandler<IniciarVendaCommand, IBusinessResult<VendaDto>>, IniciarVendaCommandHandler>();
        services.AddScoped<IMediatorCommandHandler<FinalizarVendaCommand, IBusinessResult<VendaDto>>, FinalizarVendaCommandHandler>();
        services.AddScoped<IMediatorCommandHandler<CancelarVendaCommand, IBusinessResult<bool>>, CancelarVendaCommandHandler>();

        // Query Handlers
        services.AddScoped<IMediatorQueryHandler<GetClienteByIdQuery, IBusinessResult<ClienteDto>>, GetClienteByIdQueryHandler>();
        services.AddScoped<IMediatorQueryHandler<GetAllClientesQuery, IBusinessResult<IEnumerable<ClienteDto>>>, GetAllClientesQueryHandler>();
        services.AddScoped<IMediatorQueryHandler<GetAllCategoriasQuery, IBusinessResult<IEnumerable<CategoriaDto>>>, GetAllCategoriasQueryHandler>();
        services.AddScoped<IMediatorQueryHandler<GetProdutoByIdQuery, IBusinessResult<ProdutoDto>>, GetProdutoByIdQueryHandler>();
        services.AddScoped<IMediatorQueryHandler<GetAllProdutosQuery, IBusinessResult<IEnumerable<ProdutoDto>>>, GetAllProdutosQueryHandler>();
        services.AddScoped<IMediatorQueryHandler<GetProdutosByCategoriaQuery, IBusinessResult<IEnumerable<ProdutoDto>>>, GetProdutosByCategoriaQueryHandler>();
        services.AddScoped<IMediatorQueryHandler<GetVendaByIdQuery, IBusinessResult<VendaDto>>, GetVendaByIdQueryHandler>();
        services.AddScoped<IMediatorQueryHandler<GetVendasByClienteQuery, IBusinessResult<IEnumerable<VendaResumoDto>>>, GetVendasByClienteQueryHandler>();
        services.AddScoped<IMediatorQueryHandler<GetRelatorioVendasQuery, IBusinessResult<RelatorioVendasDto>>, GetRelatorioVendasQueryHandler>();

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<CreateClienteCommandValidator>();

        // Saga Steps
        services.AddScoped<ISagaStep<ProcessarVendaSagaContext>, ValidarClienteStep>();
        services.AddScoped<ISagaStep<ProcessarVendaSagaContext>, ValidarProdutosStep>();
        services.AddScoped<ISagaStep<ProcessarVendaSagaContext>, ReservarEstoqueStep>();
        services.AddScoped<ISagaStep<ProcessarVendaSagaContext>, CriarVendaStep>();

        // Saga Orchestrator
        services.AddScoped<ProcessarVendaSaga>();

        // Hosted Services
        services.AddHostedService<HostedServices.OutboxProcessorService>();

        return services;
    }
}
