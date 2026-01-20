using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lab01.MinimalApi.Entities;

namespace Lab01.MinimalApi.Data.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Descricao)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Preco)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.Ativo)
            .HasDefaultValue(true);

        builder.Property(x => x.DataCriacao)
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(x => x.Nome);
    }
}
