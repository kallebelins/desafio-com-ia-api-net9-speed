using FluentValidation;
using Lab08.Application.Interfaces;
using Lab08.Application.UseCases.Categorias.CreateCategoria;
using Lab08.Application.UseCases.Categorias.ListCategorias;
using Lab08.Application.UseCases.Clientes.CreateCliente;
using Lab08.Application.UseCases.Clientes.GetCliente;
using Lab08.Application.UseCases.Clientes.ListClientes;
using Lab08.Application.UseCases.Produtos.CreateProduto;
using Lab08.Application.UseCases.Produtos.GetProduto;
using Lab08.Application.UseCases.Produtos.ListProdutos;
using Lab08.Application.UseCases.Vendas.ConfirmarVenda;
using Lab08.Application.UseCases.Vendas.CreateVenda;
using Lab08.Application.UseCases.Vendas.GetVenda;
using Lab08.Application.UseCases.Vendas.RelatorioVendas;
using Lab08.Application.Validators;
using Lab08.Domain.Interfaces;
using Lab08.Domain.Services;
using Lab08.Infrastructure.Data;
using Lab08.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Lab08.WebAPI.Extensions;

/// <summary>
/// Extension methods para configuração de serviços
/// </summary>
public static class ServiceBuilderExtensions
{
    /// <summary>
    /// Configura os serviços da camada de infraestrutura
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddSingleton<IDateTimeService, DateTimeService>();

        return services;
    }

    /// <summary>
    /// Configura os serviços da camada de aplicação (Use Cases)
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Domain Services
        services.AddScoped<VendaDomainService>();

        // Clientes Use Cases
        services.AddScoped<IUseCase<CreateClienteInput, CreateClienteOutput>, CreateClienteUseCase>();
        services.AddScoped<IUseCase<GetClienteInput, GetClienteOutput>, GetClienteUseCase>();
        services.AddScoped<IUseCase<ListClientesInput, ListClientesOutput>, ListClientesUseCase>();

        // Categorias Use Cases
        services.AddScoped<IUseCase<CreateCategoriaInput, CreateCategoriaOutput>, CreateCategoriaUseCase>();
        services.AddScoped<IUseCase<ListCategoriasInput, ListCategoriasOutput>, ListCategoriasUseCase>();

        // Produtos Use Cases
        services.AddScoped<IUseCase<CreateProdutoInput, CreateProdutoOutput>, CreateProdutoUseCase>();
        services.AddScoped<IUseCase<GetProdutoInput, GetProdutoOutput>, GetProdutoUseCase>();
        services.AddScoped<IUseCase<ListProdutosInput, ListProdutosOutput>, ListProdutosUseCase>();

        // Vendas Use Cases
        services.AddScoped<IUseCase<CreateVendaInput, CreateVendaOutput>, CreateVendaUseCase>();
        services.AddScoped<IUseCase<GetVendaInput, GetVendaOutput>, GetVendaUseCase>();
        services.AddScoped<IUseCase<ConfirmarVendaInput, ConfirmarVendaOutput>, ConfirmarVendaUseCase>();
        services.AddScoped<IUseCase<RelatorioVendasInput, RelatorioVendasOutput>, RelatorioVendasUseCase>();

        // Validators
        services.AddValidatorsFromAssemblyContaining<CreateClienteValidator>();

        return services;
    }

    /// <summary>
    /// Configura os serviços da WebAPI
    /// </summary>
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Lab08 Clean Architecture - Sistema de Vendas",
                Version = "v1",
                Description = "API de Sistema de Vendas usando Clean Architecture com Mvp24Hours"
            });
        });

        return services;
    }
}
