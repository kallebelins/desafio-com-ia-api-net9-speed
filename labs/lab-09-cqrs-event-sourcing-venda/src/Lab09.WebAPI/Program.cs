using Lab09.Application.Handlers.Commands;
using Lab09.Application.Handlers.Queries;
using Lab09.Application.Projections;
using Lab09.Core.Interfaces;
using Lab09.Infrastructure.EventStore;
using Lab09.Infrastructure.Projections;
using Lab09.Infrastructure.Snapshots;
using Lab09.WebAPI.Extensions;
using Lab09.WebAPI.HostedServices;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "Lab09 CQRS + Event Sourcing - Vendas", 
        Version = "v1",
        Description = "API de Vendas com Event Sourcing - Estado derivado de eventos"
    });
});

// Connection strings (usando mesmo banco para simplificar - em produção, separar)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration.GetConnectionString("EventStore")
    ?? "Server=localhost;Database=Lab09_EventSourcing;User Id=sa;Password=Lab@Mvp24Hours!;TrustServerCertificate=True;";

// Event Store DbContext
builder.Services.AddDbContext<EventStoreDbContext>(options =>
    options.UseSqlServer(connectionString));

// Projection (Read Model) DbContext
builder.Services.AddDbContext<ProjectionDbContext>(options =>
    options.UseSqlServer(connectionString));

// Snapshot DbContext
builder.Services.AddDbContext<SnapshotDbContext>(options =>
    options.UseSqlServer(connectionString));

// Event Store
builder.Services.AddScoped<IEventStore, EfCoreEventStore>();

// Snapshot Store
builder.Services.AddScoped<ISnapshotStore, SnapshotStore>();

// Projections
builder.Services.AddScoped<IProjection, VendaProjection>(sp =>
    new VendaProjection(sp.GetRequiredService<ProjectionDbContext>()));
builder.Services.AddScoped<IProjection, RelatorioVendasProjection>(sp =>
    new RelatorioVendasProjection(sp.GetRequiredService<ProjectionDbContext>()));

// Projection Engine
builder.Services.AddScoped<ProjectionEngine>();

// Register DbContext for Query Handler
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<ProjectionDbContext>());

// Command Handlers
builder.Services.AddScoped<VendaCommandHandler>();

// Query Handlers
builder.Services.AddScoped<VendaQueryHandler>();

// Mvp24Hours CQRS
builder.Services.AddMvp24HoursCqrs();

// Hosted Service para processar projeções
builder.Services.AddHostedService<ProjectionHostedService>();

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

// Ensure databases are created with all tables
using (var scope = app.Services.CreateScope())
{
    var eventStoreContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
    var projectionContext = scope.ServiceProvider.GetRequiredService<ProjectionDbContext>();
    var snapshotContext = scope.ServiceProvider.GetRequiredService<SnapshotDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Criar banco se não existir
        await eventStoreContext.Database.EnsureCreatedAsync();
        
        // Criar tabelas do Projection DbContext se não existirem
        var projectionScript = projectionContext.Database.GenerateCreateScript();
        try
        {
            // Tentar executar DDL para criar tabelas faltantes
            await projectionContext.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='VendasReadModel' and xtype='U')
                CREATE TABLE VendasReadModel (
                    Id UNIQUEIDENTIFIER PRIMARY KEY,
                    ClienteId UNIQUEIDENTIFIER NOT NULL,
                    Subtotal DECIMAL(18,2) NOT NULL,
                    Desconto DECIMAL(18,2) NOT NULL,
                    Total DECIMAL(18,2) NOT NULL,
                    Status NVARCHAR(50) NOT NULL,
                    QuantidadeItens INT NOT NULL,
                    DataInicio DATETIME2 NULL,
                    DataFinalizacao DATETIME2 NULL,
                    DataCancelamento DATETIME2 NULL,
                    Version INT NOT NULL,
                    LastUpdated DATETIME2 NOT NULL,
                    ItensJson NVARCHAR(4000) NOT NULL
                )");

            await projectionContext.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='RelatoriosVendasReadModel' and xtype='U')
                CREATE TABLE RelatoriosVendasReadModel (
                    Data DATETIME2 PRIMARY KEY,
                    TotalVendas INT NOT NULL,
                    VendasFinalizadas INT NOT NULL,
                    VendasCanceladas INT NOT NULL,
                    VendasEmAndamento INT NOT NULL,
                    ValorTotal DECIMAL(18,2) NOT NULL,
                    TotalDescontos DECIMAL(18,2) NOT NULL,
                    TicketMedio DECIMAL(18,2) NOT NULL,
                    LastUpdated DATETIME2 NOT NULL
                )");

            await projectionContext.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ProjectionCheckpoints' and xtype='U')
                CREATE TABLE ProjectionCheckpoints (
                    ProjectionName NVARCHAR(100) PRIMARY KEY,
                    LastProcessedPosition BIGINT NOT NULL,
                    LastUpdated DATETIME2 NOT NULL
                )");

            await snapshotContext.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='VendaSnapshots' and xtype='U')
                CREATE TABLE VendaSnapshots (
                    Id UNIQUEIDENTIFIER PRIMARY KEY,
                    AggregateId UNIQUEIDENTIFIER NOT NULL,
                    Version INT NOT NULL,
                    StateJson NVARCHAR(MAX) NOT NULL,
                    CreatedAt DATETIME2 NOT NULL
                )");
                
            logger.LogInformation("Todas as tabelas criadas/verificadas com sucesso");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Erro ao criar tabelas adicionais (podem já existir)");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao inicializar banco de dados");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab09 CQRS + Event Sourcing v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
