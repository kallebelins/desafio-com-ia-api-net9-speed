using Lab07.Application.Sagas;
using Lab07.Application.Sagas.Steps;
using Lab07.Application.Services;
using Lab07.Infrastructure.Data;
using Lab07.Infrastructure.Outbox;
using Lab07.WebAPI.HostedServices;
using Microsoft.EntityFrameworkCore;
using Mvp24Hours.Extensions;

namespace Lab07.WebAPI.Extensions;

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
        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddMvp24HoursDbContext<DataContext>();
        services.AddMvp24HoursRepositoryAsync(options =>
        {
            options.MaxQtyByQueryPage = 100;
        });

        return services;
    }

    /// <summary>
    /// Configura os serviços de aplicação
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IProdutoService, ProdutoService>();
        services.AddScoped<IVendaService, VendaService>();
        services.AddScoped<INotificacaoService, NotificacaoService>();

        // Outbox
        services.AddScoped<IOutboxService, OutboxService>();

        return services;
    }

    /// <summary>
    /// Configura a Saga de criação de venda
    /// </summary>
    public static IServiceCollection AddSagas(this IServiceCollection services)
    {
        // Saga Steps
        services.AddScoped<ISagaStep<CriarVendaSagaContext>, ValidarClienteStep>();
        services.AddScoped<ISagaStep<CriarVendaSagaContext>, ValidarProdutosStep>();
        services.AddScoped<ISagaStep<CriarVendaSagaContext>, ReservarEstoqueStep>();
        services.AddScoped<ISagaStep<CriarVendaSagaContext>, CriarVendaStep>();
        services.AddScoped<ISagaStep<CriarVendaSagaContext>, NotificarClienteStep>();

        // Saga Orchestrator
        services.AddScoped<CriarVendaSaga>();

        return services;
    }

    /// <summary>
    /// Configura os serviços de background
    /// </summary>
    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<OutboxProcessorService>();
        return services;
    }

    /// <summary>
    /// Configura o RabbitMQ (opcional - para uso futuro)
    /// </summary>
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        // RabbitMQ é opcional - o OutboxProcessor simula a publicação
        // Para produção, integrar com Mvp24Hours.Infrastructure.RabbitMQ
        var rabbitMQSection = configuration.GetSection("RabbitMQ");
        
        if (rabbitMQSection.Exists() && !string.IsNullOrEmpty(rabbitMQSection["HostName"]))
        {
            // Configurações de RabbitMQ disponíveis
            // A integração real com RabbitMQ pode ser adicionada aqui
        }

        return services;
    }

    /// <summary>
    /// Aplica as migrations ou cria o banco de dados
    /// </summary>
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataContext>>();

        try
        {
            logger.LogInformation("Verificando banco de dados...");
            
            // Verifica se existe migrations pendentes
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Aplicando {Count} migrations pendentes...", pendingMigrations.Count());
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations aplicadas com sucesso");
            }
            else
            {
                // Sem migrations, usa EnsureCreated para criar as tabelas
                logger.LogInformation("Nenhuma migration encontrada. Usando EnsureCreated...");
                await context.Database.EnsureCreatedAsync();
                logger.LogInformation("Banco de dados criado/verificado com sucesso");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao configurar banco de dados");
            throw;
        }
    }

    /// <summary>
    /// Seed de dados iniciais
    /// </summary>
    public static async Task SeedDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataContext>>();

        // Verifica se já existem dados
        if (await context.Clientes.AnyAsync())
        {
            logger.LogInformation("Banco já possui dados, pulando seed");
            return;
        }

        logger.LogInformation("Executando seed de dados...");

        // Clientes
        var clientes = new[]
        {
            new Lab07.Core.Entities.Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "joao.silva@email.com",
                Telefone = "(11) 99999-1111",
                Ativo = true,
                Created = DateTime.UtcNow
            },
            new Lab07.Core.Entities.Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "Maria Santos",
                Email = "maria.santos@email.com",
                Telefone = "(11) 99999-2222",
                Ativo = true,
                Created = DateTime.UtcNow
            },
            new Lab07.Core.Entities.Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "Pedro Oliveira",
                Email = "pedro.oliveira@email.com",
                Telefone = "(11) 99999-3333",
                Ativo = false,
                Created = DateTime.UtcNow
            }
        };

        await context.Clientes.AddRangeAsync(clientes);

        // Produtos
        var produtos = new[]
        {
            new Lab07.Core.Entities.Produto
            {
                Id = Guid.NewGuid(),
                Nome = "Notebook Dell Inspiron",
                Descricao = "Notebook Dell Inspiron 15, Intel Core i7, 16GB RAM, 512GB SSD",
                Preco = 4500.00m,
                Estoque = 10,
                EstoqueReservado = 0,
                Ativo = true,
                Created = DateTime.UtcNow
            },
            new Lab07.Core.Entities.Produto
            {
                Id = Guid.NewGuid(),
                Nome = "Mouse Logitech MX Master",
                Descricao = "Mouse sem fio Logitech MX Master 3S",
                Preco = 650.00m,
                Estoque = 50,
                EstoqueReservado = 0,
                Ativo = true,
                Created = DateTime.UtcNow
            },
            new Lab07.Core.Entities.Produto
            {
                Id = Guid.NewGuid(),
                Nome = "Teclado Mecânico Keychron",
                Descricao = "Teclado mecânico Keychron K2, switches Brown",
                Preco = 450.00m,
                Estoque = 30,
                EstoqueReservado = 0,
                Ativo = true,
                Created = DateTime.UtcNow
            },
            new Lab07.Core.Entities.Produto
            {
                Id = Guid.NewGuid(),
                Nome = "Monitor LG UltraWide",
                Descricao = "Monitor LG UltraWide 34\", IPS, 144Hz",
                Preco = 3200.00m,
                Estoque = 5,
                EstoqueReservado = 0,
                Ativo = true,
                Created = DateTime.UtcNow
            },
            new Lab07.Core.Entities.Produto
            {
                Id = Guid.NewGuid(),
                Nome = "Headset HyperX Cloud",
                Descricao = "Headset gamer HyperX Cloud II, 7.1 surround",
                Preco = 550.00m,
                Estoque = 0, // Sem estoque para testar cenário de falha
                EstoqueReservado = 0,
                Ativo = true,
                Created = DateTime.UtcNow
            }
        };

        await context.Produtos.AddRangeAsync(produtos);
        await context.SaveChangesAsync();

        logger.LogInformation(
            "Seed concluído: {Clientes} clientes e {Produtos} produtos criados",
            clientes.Length, produtos.Length);
    }
}
