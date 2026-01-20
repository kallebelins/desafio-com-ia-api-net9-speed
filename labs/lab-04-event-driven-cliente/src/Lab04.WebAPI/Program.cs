using Lab04.Infrastructure.Data;
using Lab04.WebAPI.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===============================================
// Add services to the container
// ===============================================

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "Lab04 Event-Driven - Clientes API", 
        Version = "v1",
        Description = "API REST com arquitetura Event-Driven usando RabbitMQ para cadastro de clientes"
    });
});

// Custom Services (Database, RabbitMQ, Events, etc.)
builder.Services.AddMyServices(builder.Configuration);

var app = builder.Build();

// ===============================================
// Configure the HTTP request pipeline
// ===============================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab04 Event-Driven v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ===============================================
// Database initialization
// ===============================================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Ensuring database is created...");
        await context.Database.EnsureCreatedAsync();
        logger.LogInformation("Database ready!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error initializing database");
    }
}

app.Run();
