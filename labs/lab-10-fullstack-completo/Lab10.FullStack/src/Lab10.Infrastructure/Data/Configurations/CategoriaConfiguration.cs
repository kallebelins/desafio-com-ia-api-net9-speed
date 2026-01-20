using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lab10.Domain.Entities;

namespace Lab10.Infrastructure.Data.Configurations;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categorias");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Descricao)
            .HasMaxLength(500);

        builder.Property(c => c.Ativo)
            .IsRequired();

        builder.Property(c => c.DataCadastro)
            .IsRequired();

        builder.HasIndex(c => c.Nome).IsUnique();

        // Ignorar coleção de produtos para evitar ciclo
        builder.Ignore(c => c.Produtos);
    }
}
