using FluentValidation;
using Lab03.Application.Handlers.Commands;
using Lab03.Application.Validators;
using Lab03.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Mvp24Hours.Extensions;
using Mvp24Hours.Infrastructure.Cqrs.Extensions;

namespace Lab03.WebAPI.Extensions;

/// <summary>
/// Extensões para configuração de serviços
/// </summary>
public static class ServiceBuilderExtensions
{
    /// <summary>
    /// Configura o banco de dados
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            }));

        // Registra o DbContext do Mvp24Hours
        services.AddMvp24HoursDbContext<DataContext>();

        // Registra o repositório assíncrono do Mvp24Hours
        services.AddMvp24HoursRepositoryAsync(options =>
        {
            options.MaxQtyByQueryPage = 100;
        });

        return services;
    }

    /// <summary>
    /// Configura o CQRS/Mediator do Mvp24Hours
    /// </summary>
    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        // Registra o Mediator do Mvp24Hours com scan do assembly dos handlers
        // NOTA: ValidationBehavior desabilitado temporariamente devido a conflito de versões
        // A validação será feita manualmente nos handlers ou controller
        services.AddMvpMediator(options =>
        {
            options.RegisterHandlersFromAssemblyContaining<CreateProdutoCommandHandler>();
            options.RegisterValidationBehavior = false; // Desabilitado - conflito de versões
            options.RegisterLoggingBehavior = true;
        });

        return services;
    }

    /// <summary>
    /// Configura os validadores FluentValidation
    /// </summary>
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateProdutoValidator>();

        return services;
    }

    /// <summary>
    /// Inicializa o banco de dados
    /// </summary>
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Garante que o banco de dados está criado
        await context.Database.EnsureCreatedAsync();
    }
}
