using Lab09.Application.Projections;
using Microsoft.EntityFrameworkCore;

namespace Lab09.Infrastructure.Projections;

/// <summary>
/// DbContext para os Read Models (projeções)
/// </summary>
public class ProjectionDbContext : DbContext
{
    public ProjectionDbContext(DbContextOptions<ProjectionDbContext> options) : base(options)
    {
    }

    public DbSet<VendaReadModel> Vendas { get; set; } = null!;
    public DbSet<RelatorioVendasReadModel> RelatoriosVendas { get; set; } = null!;
    public DbSet<ProjectionCheckpoint> Checkpoints { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // VendaReadModel
        modelBuilder.Entity<VendaReadModel>(entity =>
        {
            entity.ToTable("VendasReadModel");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.ClienteId).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Subtotal).HasPrecision(18, 2);
            entity.Property(e => e.Desconto).HasPrecision(18, 2);
            entity.Property(e => e.Total).HasPrecision(18, 2);
            entity.Property(e => e.ItensJson).HasMaxLength(4000);
            
            entity.HasIndex(e => e.ClienteId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.DataInicio);
        });

        // RelatorioVendasReadModel
        modelBuilder.Entity<RelatorioVendasReadModel>(entity =>
        {
            entity.ToTable("RelatoriosVendasReadModel");
            entity.HasKey(e => e.Data);
            
            entity.Property(e => e.ValorTotal).HasPrecision(18, 2);
            entity.Property(e => e.TotalDescontos).HasPrecision(18, 2);
            entity.Property(e => e.TicketMedio).HasPrecision(18, 2);
        });

        // ProjectionCheckpoint
        modelBuilder.Entity<ProjectionCheckpoint>(entity =>
        {
            entity.ToTable("ProjectionCheckpoints");
            entity.HasKey(e => e.ProjectionName);
            
            entity.Property(e => e.ProjectionName).HasMaxLength(100);
        });
    }
}

/// <summary>
/// Checkpoint para controle de posição das projeções
/// </summary>
public class ProjectionCheckpoint
{
    public string ProjectionName { get; set; } = string.Empty;
    public long LastProcessedPosition { get; set; }
    public DateTime LastUpdated { get; set; }
}
