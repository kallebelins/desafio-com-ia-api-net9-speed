using Microsoft.EntityFrameworkCore;

namespace Lab09.Infrastructure.EventStore;

/// <summary>
/// DbContext para o Event Store
/// </summary>
public class EventStoreDbContext : DbContext
{
    public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options) : base(options)
    {
    }

    public DbSet<StoredEvent> StoredEvents { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StoredEvent>(entity =>
        {
            entity.ToTable("StoredEvents");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
            
            entity.Property(e => e.EventId)
                .IsRequired();
            
            entity.Property(e => e.AggregateId)
                .IsRequired();
            
            entity.Property(e => e.AggregateType)
                .HasMaxLength(256)
                .IsRequired();
            
            entity.Property(e => e.EventType)
                .HasMaxLength(512)
                .IsRequired();
            
            entity.Property(e => e.EventData)
                .IsRequired();
            
            entity.Property(e => e.Version)
                .IsRequired();
            
            entity.Property(e => e.Timestamp)
                .IsRequired();
            
            entity.Property(e => e.UserId)
                .HasMaxLength(100);
            
            entity.Property(e => e.Metadata)
                .HasMaxLength(2000);

            // Índice único para garantir versão por aggregate
            entity.HasIndex(e => new { e.AggregateId, e.Version })
                .IsUnique();
            
            // Índice para busca por aggregate
            entity.HasIndex(e => e.AggregateId);
            
            // Índice para busca por timestamp (projeções)
            entity.HasIndex(e => e.Id);
        });
    }
}
