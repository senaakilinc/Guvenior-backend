using Güvenior.Domain.Common;
using Güvenior.Domain.Enums;

namespace Güvenior.Domain.Entities;

public class FinancialGoal : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public FinancialGoalType Type { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal CurrentSavings { get; set; }
    public decimal MonthlyContribution { get; set; }
    public decimal AnnualInflationRate { get; set; }
    public DateTime TargetDate { get; set; }
    public bool IsCompleted { get; set; }
}
