using Lab06.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// Alias para evitar conflito com namespace Lab06.Infrastructure.Adapters.Outbound.Email
using DomainEmail = Lab06.Domain.ValueObjects.Email;
using DomainCPF = Lab06.Domain.ValueObjects.CPF;

namespace Lab06.Infrastructure.Adapters.Outbound.Persistence.Configurations;

/// <summary>
/// Configuração do EF Core para a entidade Cliente
/// </summary>
public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        // Configuração do Value Object Email
        builder.Property(c => c.Email)
            .HasConversion(
                email => email.Value,
                value => DomainEmail.Create(value))
            .IsRequired()
            .HasMaxLength(256);

        // Configuração do Value Object CPF
        builder.Property(c => c.Cpf)
            .HasConversion(
                cpf => cpf.Value,
                value => DomainCPF.Create(value))
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(c => c.Telefone)
            .HasMaxLength(11);

        builder.Property(c => c.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.DataCriacao)
            .IsRequired();

        builder.Property(c => c.DataAtualizacao);

        // Configuração do Value Object Endereco como Owned Type
        builder.OwnsOne(c => c.Endereco, endereco =>
        {
            endereco.Property(e => e.Logradouro)
                .HasColumnName("Logradouro")
                .HasMaxLength(200);

            endereco.Property(e => e.Numero)
                .HasColumnName("Numero")
                .HasMaxLength(20);

            endereco.Property(e => e.Complemento)
                .HasColumnName("Complemento")
                .HasMaxLength(100);

            endereco.Property(e => e.Bairro)
                .HasColumnName("Bairro")
                .HasMaxLength(100);

            endereco.Property(e => e.Cidade)
                .HasColumnName("Cidade")
                .HasMaxLength(100);

            endereco.Property(e => e.Estado)
                .HasColumnName("Estado")
                .HasMaxLength(2);

            endereco.Property(e => e.CEP)
                .HasColumnName("CEP")
                .HasMaxLength(8);
        });

        // Índices
        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.HasIndex(c => c.Cpf)
            .IsUnique();

        builder.HasIndex(c => c.Ativo);
    }
}
