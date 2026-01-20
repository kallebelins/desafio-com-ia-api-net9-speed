using Lab03.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lab03.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade Produto para o EF Core
/// </summary>
public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Descricao)
            .HasMaxLength(1000);

        builder.Property(p => p.Preco)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Categoria)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Estoque)
            .IsRequired();

        builder.Property(p => p.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.Created)
            .IsRequired();

        builder.Property(p => p.Modified);

        // Índices
        builder.HasIndex(p => p.Categoria);
        builder.HasIndex(p => p.Ativo);
        builder.HasIndex(p => p.Nome);
    }
}
