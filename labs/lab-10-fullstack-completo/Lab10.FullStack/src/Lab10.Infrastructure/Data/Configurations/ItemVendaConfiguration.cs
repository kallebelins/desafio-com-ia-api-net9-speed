using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lab10.Domain.Entities;

namespace Lab10.Infrastructure.Data.Configurations;

public class ItemVendaConfiguration : IEntityTypeConfiguration<ItemVenda>
{
    public void Configure(EntityTypeBuilder<ItemVenda> builder)
    {
        builder.ToTable("ItensVenda");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedOnAdd();

        builder.Property(i => i.ProdutoNome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(i => i.Quantidade)
            .IsRequired();

        // Value Object Money como Owned Type
        builder.OwnsOne(i => i.PrecoUnitario, preco =>
        {
            preco.Property(m => m.Valor)
                .HasColumnName("PrecoUnitario")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            preco.Property(m => m.Moeda)
                .HasColumnName("Moeda")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Relacionamento com Produto
        builder.HasOne(i => i.Produto)
            .WithMany()
            .HasForeignKey(i => i.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignorar propriedade calculada
        builder.Ignore(i => i.ValorTotal);

        // Índices
        builder.HasIndex(i => i.VendaId);
        builder.HasIndex(i => i.ProdutoId);
    }
}
