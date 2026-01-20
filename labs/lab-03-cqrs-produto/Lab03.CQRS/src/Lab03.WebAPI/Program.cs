using Lab03.WebAPI.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Lab03 CQRS - Produtos",
        Version = "v1",
        Description = "API REST para cadastro de produtos usando CQRS com Mvp24Hours",
        Contact = new OpenApiContact
        {
            Name = "Lab03 CQRS",
            Email = "lab03@example.com"
        }
    });
});

// Database
builder.Services.AddDatabase(builder.Configuration);

// CQRS/Mediator
builder.Services.AddCqrs();

// Validation
builder.Services.AddValidation();

var app = builder.Build();

// Initialize database
await app.Services.InitializeDatabaseAsync();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab03 CQRS v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
