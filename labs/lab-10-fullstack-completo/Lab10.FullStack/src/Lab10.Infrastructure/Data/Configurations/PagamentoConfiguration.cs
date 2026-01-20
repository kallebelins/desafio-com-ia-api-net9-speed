using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lab10.Domain.Entities;

namespace Lab10.Infrastructure.Data.Configurations;

public class PagamentoConfiguration : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        builder.ToTable("Pagamentos");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.Metodo)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.TransacaoId)
            .HasMaxLength(100);

        builder.Property(p => p.DataCriacao)
            .IsRequired();

        builder.Property(p => p.MotivoFalha)
            .HasMaxLength(500);

        // Value Object Money como Owned Type
        builder.OwnsOne(p => p.Valor, valor =>
        {
            valor.Property(m => m.Valor)
                .HasColumnName("Valor")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            valor.Property(m => m.Moeda)
                .HasColumnName("Moeda")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Índices
        builder.HasIndex(p => p.VendaId).IsUnique();
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.TransacaoId);
    }
}
