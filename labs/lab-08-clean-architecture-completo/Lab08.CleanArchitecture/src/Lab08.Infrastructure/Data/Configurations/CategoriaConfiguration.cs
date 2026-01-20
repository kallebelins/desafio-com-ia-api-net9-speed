using Lab08.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lab08.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração EF Core para Categoria
/// </summary>
public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categorias");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(c => c.Nome)
            .IsUnique();

        builder.Property(c => c.Descricao)
            .HasMaxLength(500);

        builder.Property(c => c.Ativo)
            .IsRequired();

        // Relacionamentos
        builder.HasMany(c => c.Produtos)
            .WithOne(p => p.Categoria)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
