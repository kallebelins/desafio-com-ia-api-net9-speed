using Lab07.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lab07.Infrastructure.Data.Configurations;

public class VendaConfiguration : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.ToTable("Vendas");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.ClienteId)
            .IsRequired();

        builder.Property(v => v.ValorTotal)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(v => v.Status)
            .IsRequired();

        builder.Property(v => v.MotivoFalha)
            .HasMaxLength(500);

        builder.Property(v => v.Created)
            .IsRequired();

        builder.HasOne(v => v.Cliente)
            .WithMany(c => c.Vendas)
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => v.ClienteId);
        builder.HasIndex(v => v.Status);
        builder.HasIndex(v => v.Created);
    }
}
