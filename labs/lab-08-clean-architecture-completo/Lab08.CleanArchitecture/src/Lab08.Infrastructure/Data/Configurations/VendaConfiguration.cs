using Lab08.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lab08.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração EF Core para Venda
/// </summary>
public class VendaConfiguration : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.ToTable("Vendas");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.DataVenda)
            .IsRequired();

        builder.Property(v => v.Status)
            .IsRequired()
            .HasConversion<int>();

        // Value Object Money para Total
        builder.OwnsOne(v => v.Total, total =>
        {
            total.Property(m => m.Valor)
                .HasColumnName("Total")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            total.Property(m => m.Moeda)
                .HasColumnName("Moeda")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(v => v.Observacao)
            .HasMaxLength(500);

        // Índices
        builder.HasIndex(v => v.ClienteId);
        builder.HasIndex(v => v.DataVenda);
        builder.HasIndex(v => v.Status);

        // Relacionamentos
        builder.HasOne(v => v.Cliente)
            .WithMany(c => c.Vendas)
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuração para a lista de itens (campo privado _itens)
        builder.HasMany(v => v.Itens)
            .WithOne(i => i.Venda)
            .HasForeignKey(i => i.VendaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Acessar o campo privado _itens
        builder.Navigation(v => v.Itens)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
