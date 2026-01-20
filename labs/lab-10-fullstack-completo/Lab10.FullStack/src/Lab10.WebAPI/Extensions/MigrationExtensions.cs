using Microsoft.EntityFrameworkCore;
using Lab10.Infrastructure.Data;

namespace Lab10.WebAPI.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<WriteDbContext>>();

        try
        {
            logger.LogInformation("Verificando banco de dados...");

            // Verificar se há migrations pendentes
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Aplicando {Count} migrations pendentes...", pendingMigrations.Count());
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations aplicadas com sucesso");
            }
            else
            {
                // Sem migrations, usa EnsureCreated
                logger.LogInformation("Nenhuma migration encontrada. Usando EnsureCreated...");
                await context.Database.EnsureCreatedAsync();
                logger.LogInformation("Banco de dados criado/verificado com sucesso");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao aplicar migrations");
            throw;
        }
    }
}
