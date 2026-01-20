using FluentValidation;
using Lab06.Application.Ports.Inbound;
using Lab06.Application.Ports.Outbound;
using Lab06.Application.UseCases;
using Lab06.Application.Validators;
using Lab06.Infrastructure.Adapters.Outbound.Email;
using Lab06.Infrastructure.Adapters.Outbound.ExternalServices;
using Lab06.Infrastructure.Adapters.Outbound.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lab06.WebAPI.Extensions;

/// <summary>
/// Extension methods para configuração dos serviços da aplicação
/// </summary>
public static class ServiceBuilderExtensions
{
    /// <summary>
    /// Registra todos os serviços da aplicação no container DI
    /// </summary>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(DataContext).Assembly.FullName)));

        // Validators
        services.AddValidatorsFromAssemblyContaining<CreateClienteValidator>();

        // Outbound Adapters (Secondary/Driven)
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<ICpfValidationService, CpfValidationService>();

        // Inbound Ports (Use Cases)
        services.AddScoped<ICreateClienteUseCase, CreateClienteUseCase>();
        services.AddScoped<IGetClienteUseCase, GetClienteUseCase>();
        services.AddScoped<IUpdateClienteUseCase, UpdateClienteUseCase>();
        services.AddScoped<IDeleteClienteUseCase, DeleteClienteUseCase>();

        return services;
    }

    /// <summary>
    /// Configura o banco de dados (recria em desenvolvimento para garantir schema correto)
    /// </summary>
    public static async Task ConfigureDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataContext>>();

        try
        {
            logger.LogInformation("🔄 Configurando banco de dados...");
            
            // Recria o banco de dados para garantir que a estrutura está correta
            // Em produção, usar migrações!
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            
            logger.LogInformation("✅ Banco de dados configurado com sucesso!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Falha ao configurar banco de dados");
            throw;
        }
    }
}
