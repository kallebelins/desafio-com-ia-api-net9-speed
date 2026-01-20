using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lab10.Domain.Entities;

namespace Lab10.Infrastructure.Data.Configurations;

public class VendaConfiguration : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.ToTable("Vendas");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).ValueGeneratedOnAdd();

        builder.Property(v => v.Status)
            .IsRequired();

        builder.Property(v => v.DataCriacao)
            .IsRequired();

        builder.Property(v => v.Observacao)
            .HasMaxLength(500);

        // Value Object Money como Owned Type
        builder.OwnsOne(v => v.ValorTotal, valor =>
        {
            valor.Property(m => m.Valor)
                .HasColumnName("ValorTotal")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            valor.Property(m => m.Moeda)
                .HasColumnName("Moeda")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Relacionamento com Cliente
        builder.HasOne(v => v.Cliente)
            .WithMany()
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento com Itens
        builder.HasMany(v => v.Itens)
            .WithOne(i => i.Venda)
            .HasForeignKey(i => i.VendaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento com Pagamento
        builder.HasOne(v => v.Pagamento)
            .WithOne(p => p.Venda)
            .HasForeignKey<Pagamento>(p => p.VendaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(v => v.ClienteId);
        builder.HasIndex(v => v.Status);
        builder.HasIndex(v => v.DataCriacao);
    }
}
