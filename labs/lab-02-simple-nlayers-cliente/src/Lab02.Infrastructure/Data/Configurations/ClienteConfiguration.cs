using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lab02.Core.Entities;

namespace Lab02.Infrastructure.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Telefone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Ativo)
            .HasDefaultValue(true);

        builder.Property(x => x.DataCriacao)
            .IsRequired();

        builder.HasIndex(x => x.Email).IsUnique();
    }
}
