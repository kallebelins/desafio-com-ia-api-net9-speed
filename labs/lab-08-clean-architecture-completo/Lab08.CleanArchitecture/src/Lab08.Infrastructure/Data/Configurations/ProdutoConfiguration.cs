using Lab08.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lab08.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração EF Core para Produto
/// </summary>
public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(p => p.Nome)
            .IsUnique();

        builder.Property(p => p.Descricao)
            .HasMaxLength(1000);

        // Value Object Money para Preço
        builder.OwnsOne(p => p.Preco, preco =>
        {
            preco.Property(m => m.Valor)
                .HasColumnName("Preco")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            preco.Property(m => m.Moeda)
                .HasColumnName("Moeda")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(p => p.Estoque)
            .IsRequired();

        builder.Property(p => p.Ativo)
            .IsRequired();

        builder.Property(p => p.DataCadastro)
            .IsRequired();

        builder.Property(p => p.DataAtualizacao);

        // Índice para categoria
        builder.HasIndex(p => p.CategoriaId);

        // Relacionamentos
        builder.HasOne(p => p.Categoria)
            .WithMany(c => c.Produtos)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.ItensVenda)
            .WithOne(i => i.Produto)
            .HasForeignKey(i => i.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
