using Lab08.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lab08.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração EF Core para ItemVenda
/// </summary>
public class ItemVendaConfiguration : IEntityTypeConfiguration<ItemVenda>
{
    public void Configure(EntityTypeBuilder<ItemVenda> builder)
    {
        builder.ToTable("ItensVenda");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Quantidade)
            .IsRequired();

        // Value Object Money para PrecoUnitario
        builder.OwnsOne(i => i.PrecoUnitario, preco =>
        {
            preco.Property(m => m.Valor)
                .HasColumnName("PrecoUnitario")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            preco.Property(m => m.Moeda)
                .HasColumnName("MoedaPreco")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Value Object Money para Subtotal
        builder.OwnsOne(i => i.Subtotal, subtotal =>
        {
            subtotal.Property(m => m.Valor)
                .HasColumnName("Subtotal")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            subtotal.Property(m => m.Moeda)
                .HasColumnName("MoedaSubtotal")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Índices
        builder.HasIndex(i => i.VendaId);
        builder.HasIndex(i => i.ProdutoId);
        builder.HasIndex(i => new { i.VendaId, i.ProdutoId })
            .IsUnique();

        // Relacionamentos
        builder.HasOne(i => i.Venda)
            .WithMany(v => v.Itens)
            .HasForeignKey(i => i.VendaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Produto)
            .WithMany(p => p.ItensVenda)
            .HasForeignKey(i => i.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
