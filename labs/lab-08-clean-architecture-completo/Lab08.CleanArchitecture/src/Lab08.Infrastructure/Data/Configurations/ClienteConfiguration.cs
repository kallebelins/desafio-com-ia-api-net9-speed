using Lab08.Domain.Entities;
using Lab08.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lab08.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração EF Core para Cliente
/// </summary>
public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        // Value Object Email
        builder.OwnsOne(c => c.Email, email =>
        {
            email.Property(e => e.Valor)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(256);

            email.HasIndex(e => e.Valor)
                .IsUnique();
        });

        // Value Object CPF
        builder.OwnsOne(c => c.Cpf, cpf =>
        {
            cpf.Property(c => c.Valor)
                .HasColumnName("Cpf")
                .IsRequired()
                .HasMaxLength(11);

            cpf.HasIndex(c => c.Valor)
                .IsUnique();
        });

        // Value Object Endereco (opcional)
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

            endereco.Property(e => e.Cep)
                .HasColumnName("Cep")
                .HasMaxLength(8);
        });

        builder.Property(c => c.Ativo)
            .IsRequired();

        builder.Property(c => c.DataCadastro)
            .IsRequired();

        builder.Property(c => c.DataAtualizacao);

        // Relacionamentos
        builder.HasMany(c => c.Vendas)
            .WithOne(v => v.Cliente)
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
