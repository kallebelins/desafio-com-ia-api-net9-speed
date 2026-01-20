using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Mvp24Hours.Extensions;
using Mvp24Hours.Infrastructure.Data.EFCore;
using Lab01.MinimalApi.Data;
using Lab01.MinimalApi.Validators;
using Lab01.MinimalApi.ValueObjects;

namespace Lab01.MinimalApi.Extensions;

public static class ServiceBuilderExtensions
{
    public static IServiceCollection AddMyServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null)));

        // Mvp24Hours Services
        services.AddMvp24HoursDbContext<DataContext>();
        services.AddMvp24HoursRepositoryAsync();

        // Validators
        services.AddScoped<IValidator<ProdutoCreateDto>, ProdutoCreateValidator>();
        services.AddScoped<IValidator<ProdutoUpdateDto>, ProdutoUpdateValidator>();

        // Health Checks
        services.AddHealthChecks()
            .AddSqlServer(
                configuration.GetConnectionString("DefaultConnection")!,
                name: "sqlserver",
                tags: new[] { "db", "sql" });

        return services;
    }
}
