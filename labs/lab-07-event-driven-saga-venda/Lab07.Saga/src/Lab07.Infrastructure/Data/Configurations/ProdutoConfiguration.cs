using Lab07.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lab07.Infrastructure.Data.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Descricao)
            .HasMaxLength(1000);

        builder.Property(p => p.Preco)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Estoque)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.EstoqueReservado)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.Created)
            .IsRequired();

        // Ignore computed property
        builder.Ignore(p => p.EstoqueDisponivel);

        builder.HasIndex(p => p.Nome);
        builder.HasIndex(p => p.Ativo);
    }
}
