using FluentValidation;
using Lab05.Application.Commands;
using Lab05.Application.Handlers;
using Lab05.Application.Infrastructure;
using Lab05.Application.Metrics;
using Lab05.Application.Queries;
using Lab05.Application.Validators;
using Lab05.Core.Entities;
using Lab05.Core.ValueObjects;
using Lab05.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Extensions;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab05.WebAPI.Extensions;

/// <summary>
/// Extensions para configuração de serviços
/// </summary>
public static class ServiceBuilderExtensions
{
    /// <summary>
    /// Configura os serviços de banco de dados
    /// </summary>
    public static IServiceCollection AddDatabaseServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddMvp24HoursDbContext<DataContext>();
        services.AddMvp24HoursRepositoryAsync(options =>
        {
            options.MaxQtyByQueryPage = 100;
        });

        return services;
    }

    /// <summary>
    /// Configura os serviços CQRS
    /// </summary>
    public static IServiceCollection AddCqrsServices(this IServiceCollection services)
    {
        // Registrar Mediator
        services.AddScoped<IMediator, SimpleMediator>();

        // Registrar Command Handlers
        services.AddScoped<IMediatorCommandHandler<CreateProdutoCommand, IBusinessResult<ProdutoDto>>, CreateProdutoCommandHandler>();
        services.AddScoped<IMediatorCommandHandler<UpdateProdutoCommand, IBusinessResult<ProdutoDto>>, UpdateProdutoCommandHandler>();
        services.AddScoped<IMediatorCommandHandler<DeleteProdutoCommand, IBusinessResult<bool>>, DeleteProdutoCommandHandler>();

        // Registrar Query Handlers
        services.AddScoped<IMediatorQueryHandler<GetProdutoByIdQuery, IBusinessResult<ProdutoDto>>, GetProdutoByIdQueryHandler>();
        services.AddScoped<IMediatorQueryHandler<GetProdutosQuery, IBusinessResult<IList<ProdutoDto>>>, GetProdutosQueryHandler>();
        services.AddScoped<IMediatorQueryHandler<GetProdutoBySKUQuery, IBusinessResult<ProdutoDto>>, GetProdutoBySKUQueryHandler>();

        // Registrar Validators
        services.AddValidatorsFromAssemblyContaining<CreateProdutoCommandValidator>();

        // Registrar Metrics
        services.AddSingleton<ProdutoMetrics>();

        return services;
    }

    /// <summary>
    /// Configura os Health Checks
    /// </summary>
    public static IServiceCollection AddHealthCheckServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddHealthChecks()
            .AddSqlServer(
                connectionString!,
                healthQuery: "SELECT 1;",
                name: "sqlserver",
                failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
                tags: new[] { "db", "sql", "ready" });

        return services;
    }

    /// <summary>
    /// Garante que o banco de dados está criado
    /// </summary>
    public static async Task EnsureDatabaseCreatedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        try
        {
            await context.Database.EnsureCreatedAsync();
            app.Logger.LogInformation("Database created/verified successfully");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Error creating database");
        }
    }
}
