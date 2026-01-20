using Lab07.WebAPI.Extensions;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("Iniciando aplicação...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // NLog
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new()
        {
            Title = "Lab07 Event-Driven + Saga - Vendas",
            Version = "v1",
            Description = "API de vendas com Saga Pattern para orquestração de transações distribuídas"
        });
    });

    // Controllers
    builder.Services.AddControllers();

    // Database
    builder.Services.AddDatabase(builder.Configuration);

    // Application Services
    builder.Services.AddApplicationServices();

    // Sagas
    builder.Services.AddSagas();

    // Background Services
    builder.Services.AddBackgroundServices();

    // Messaging (opcional)
    builder.Services.AddMessaging(builder.Configuration);

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab07 Event-Driven + Saga v1");
            c.RoutePrefix = "swagger";
        });
    }

    app.UseCors();
    app.UseAuthorization();
    app.MapControllers();

    // Health check endpoint
    app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

    // Apply migrations and seed
    await app.ApplyMigrationsAsync();
    await app.SeedDataAsync();

    logger.Info("Aplicação iniciada com sucesso");
    logger.Info("Swagger disponível em: /swagger");

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Aplicação encerrada devido a uma exceção");
    throw;
}
finally
{
    LogManager.Shutdown();
}
