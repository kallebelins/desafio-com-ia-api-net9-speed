using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lab10.Domain.Entities;

namespace Lab10.Infrastructure.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Ativo)
            .IsRequired();

        builder.Property(c => c.DataCadastro)
            .IsRequired();

        // Value Object Email como Owned Type
        builder.OwnsOne(c => c.Email, email =>
        {
            email.Property(e => e.Valor)
                .HasColumnName("Email")
                .HasMaxLength(256)
                .IsRequired();

            email.HasIndex(e => e.Valor).IsUnique();
        });

        // Value Object CPF como Owned Type
        builder.OwnsOne(c => c.Cpf, cpf =>
        {
            cpf.Property(c => c.Valor)
                .HasColumnName("Cpf")
                .HasMaxLength(11)
                .IsRequired();

            cpf.HasIndex(c => c.Valor).IsUnique();
        });

        // Value Object Endereco como Owned Type
        builder.OwnsOne(c => c.Endereco, endereco =>
        {
            endereco.Property(e => e.Logradouro)
                .HasColumnName("Endereco_Logradouro")
                .HasMaxLength(200);

            endereco.Property(e => e.Numero)
                .HasColumnName("Endereco_Numero")
                .HasMaxLength(20);

            endereco.Property(e => e.Complemento)
                .HasColumnName("Endereco_Complemento")
                .HasMaxLength(100);

            endereco.Property(e => e.Bairro)
                .HasColumnName("Endereco_Bairro")
                .HasMaxLength(100);

            endereco.Property(e => e.Cidade)
                .HasColumnName("Endereco_Cidade")
                .HasMaxLength(100);

            endereco.Property(e => e.Estado)
                .HasColumnName("Endereco_Estado")
                .HasMaxLength(2);

            endereco.Property(e => e.Cep)
                .HasColumnName("Endereco_Cep")
                .HasMaxLength(8);
        });

        // Ignorar propriedade de navegação para evitar ciclo
        builder.Ignore(c => c.Vendas);
    }
}
