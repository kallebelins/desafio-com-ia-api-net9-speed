using Lab06.WebAPI.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Lab06 Hexagonal - Clientes API",
        Version = "v1",
        Description = "API REST para cadastro de clientes usando Arquitetura Hexagonal (Ports & Adapters) com Mvp24Hours Framework",
        Contact = new OpenApiContact
        {
            Name = "Lab06 Hexagonal",
            Email = "lab06@example.com"
        }
    });
});

// Application Services (Use Cases, Repositories, etc.)
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab06 Hexagonal v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Configure Database
await app.ConfigureDatabaseAsync();

// Log startup info
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("🚀 Lab06 Hexagonal API iniciada!");
logger.LogInformation("📚 Swagger UI disponível em: /swagger");

app.Run();
