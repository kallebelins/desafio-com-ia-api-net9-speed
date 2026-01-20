using Microsoft.EntityFrameworkCore;

namespace Lab09.Infrastructure.Snapshots;

/// <summary>
/// DbContext para snapshots
/// </summary>
public class SnapshotDbContext : DbContext
{
    public SnapshotDbContext(DbContextOptions<SnapshotDbContext> options) : base(options)
    {
    }

    public DbSet<VendaSnapshot> VendaSnapshots { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<VendaSnapshot>(entity =>
        {
            entity.ToTable("VendaSnapshots");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.AggregateId).IsRequired();
            entity.Property(e => e.Version).IsRequired();
            entity.Property(e => e.StateJson).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => new { e.AggregateId, e.Version })
                .IsUnique();
            
            entity.HasIndex(e => e.AggregateId);
        });
    }
}
