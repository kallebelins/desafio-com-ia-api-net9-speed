using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Mvp24Hours.Extensions;
using Lab02.Core.Validators;
using Lab02.Infrastructure.Data;

namespace Lab02.WebAPI.Extensions;

public static class ServiceBuilderExtensions
{
    public static IServiceCollection AddMyServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        // Mvp24Hours Services
        services.AddMvp24HoursDbContext<DataContext>();
        services.AddMvp24HoursRepositoryAsync();

        // Validators
        services.AddValidatorsFromAssemblyContaining<ClienteCreateValidator>();

        // Health Checks
        services.AddHealthChecks()
            .AddSqlServer(
                configuration.GetConnectionString("DefaultConnection")!,
                name: "sqlserver",
                tags: new[] { "db", "sql" });

        return services;
    }
}
