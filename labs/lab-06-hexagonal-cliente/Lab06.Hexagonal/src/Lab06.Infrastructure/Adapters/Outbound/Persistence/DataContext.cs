using Lab06.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab06.Infrastructure.Adapters.Outbound.Persistence;

/// <summary>
/// DbContext do EF Core para a aplicação
/// </summary>
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
    }
}
