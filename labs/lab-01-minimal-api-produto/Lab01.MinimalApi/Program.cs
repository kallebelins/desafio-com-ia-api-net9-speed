using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Mvp24Hours.Extensions;
using NLog;
using NLog.Web;
using Lab01.MinimalApi.Data;
using Lab01.MinimalApi.Extensions;
using Lab01.MinimalApi.Endpoints;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    logger.Info("Starting application");

    var builder = WebApplication.CreateBuilder(args);

    // Logging
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Services
    builder.Services.AddMyServices(builder.Configuration);

    // Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "Lab01 Minimal API - Produtos", Version = "v1" });
    });

    var app = builder.Build();

    // Ensure database is created (with retry logic)
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            logger.Info("Attempting to create database if not exists...");
            dbContext.Database.EnsureCreated();
            logger.Info("Database check completed successfully");
        }
    }
    catch (Exception ex)
    {
        logger.Warn(ex, "Failed to connect to database during startup. Application will continue but database operations may fail.");
        logger.Warn("Please ensure SQL Server is running and accessible at: {ConnectionString}", 
            builder.Configuration.GetConnectionString("DefaultConnection"));
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab01 Minimal API v1"));
    }

    app.UseHttpsRedirection();

    // Health Checks
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    // Endpoints
    app.MapProdutoEndpoints();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Application stopped due to exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
