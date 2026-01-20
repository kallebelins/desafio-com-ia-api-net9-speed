using Lab04.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Mvp24Hours.Infrastructure.Data.EFCore;

namespace Lab04.Infrastructure.Data;

/// <summary>
/// Contexto de dados EF Core para o Lab04
/// </summary>
public class DataContext : Mvp24HoursContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.CPF)
                .IsRequired()
                .HasMaxLength(14);
            
            entity.Property(e => e.Telefone)
                .HasMaxLength(20);
            
            entity.Property(e => e.Ativo)
                .HasDefaultValue(true);
            
            entity.Property(e => e.DataCriacao)
                .HasDefaultValueSql("GETUTCDATE()");
            
            entity.Property(e => e.DataAtualizacao)
                .IsRequired(false);
            
            // Índices
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.CPF).IsUnique();
        });
    }
}
