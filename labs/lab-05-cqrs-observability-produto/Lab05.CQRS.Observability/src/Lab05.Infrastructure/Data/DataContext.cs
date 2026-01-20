using Lab05.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Mvp24Hours.Infrastructure.Data.EFCore;

namespace Lab05.Infrastructure.Data;

/// <summary>
/// DataContext para acesso ao banco de dados
/// </summary>
public class DataContext : Mvp24HoursContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public DbSet<Produto> Produtos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Produto>(entity =>
        {
            entity.ToTable("Produtos");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedNever();

            entity.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Descricao)
                .HasMaxLength(1000);

            entity.Property(e => e.Preco)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(e => e.SKU)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(e => e.SKU)
                .IsUnique();

            entity.Property(e => e.Categoria)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.Categoria);

            entity.Property(e => e.Ativo)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.DataCriacao)
                .IsRequired();

            entity.Property(e => e.DataAtualizacao)
                .IsRequired(false);

            entity.Property(e => e.DataRemocao)
                .IsRequired(false);
        });
    }
}
