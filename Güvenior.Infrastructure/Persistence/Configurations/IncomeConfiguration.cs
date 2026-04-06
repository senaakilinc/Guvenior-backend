using Güvenior.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Güvenior.Infrastructure.Persistence.Configurations;

public class IncomeConfiguration : IEntityTypeConfiguration<Income>
{
    public void Configure(EntityTypeBuilder<Income> builder)
    {
        builder.ToTable("Incomes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.ReceivedDate)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Incomes)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}