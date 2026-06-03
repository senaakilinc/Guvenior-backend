using Güvenior.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Güvenior.Infrastructure.Persistence.Configurations;

public class FinancialGoalConfiguration : IEntityTypeConfiguration<FinancialGoal>
{
    public void Configure(EntityTypeBuilder<FinancialGoal> builder)
    {
        builder.ToTable("FinancialGoals");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.CurrentPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.CurrentSavings)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.MonthlyContribution)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.AnnualInflationRate)
            .HasColumnType("decimal(5,2)");

        builder.Property(x => x.TargetDate)
            .IsRequired();

        builder.Property(x => x.IsCompleted)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.FinancialGoals)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
