using Lab07.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lab07.Infrastructure.Data.Configurations;

public class SagaStateConfiguration : IEntityTypeConfiguration<SagaState>
{
    public void Configure(EntityTypeBuilder<SagaState> builder)
    {
        builder.ToTable("SagaStates");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SagaType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Status)
            .IsRequired();

        builder.Property(s => s.CurrentStep)
            .HasMaxLength(100);

        builder.Property(s => s.Data)
            .IsRequired();

        builder.Property(s => s.StartedAt)
            .IsRequired();

        builder.Property(s => s.ErrorMessage)
            .HasMaxLength(2000);

        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.StartedAt);
    }
}
