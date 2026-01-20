using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lab10.Domain.Entities;

namespace Lab10.Infrastructure.Data.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Descricao)
            .HasMaxLength(500);

        builder.Property(p => p.EstoqueAtual)
            .IsRequired();

        builder.Property(p => p.EstoqueReservado)
            .IsRequired();

        builder.Property(p => p.Ativo)
            .IsRequired();

        builder.Property(p => p.DataCadastro)
            .IsRequired();

        // Value Object Money como Owned Type
        builder.OwnsOne(p => p.PrecoUnitario, preco =>
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

        // Relacionamento com Categoria
        builder.HasOne(p => p.Categoria)
            .WithMany()
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignorar propriedade calculada
        builder.Ignore(p => p.EstoqueDisponivel);
    }
}
