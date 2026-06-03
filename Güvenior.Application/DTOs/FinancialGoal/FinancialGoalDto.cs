using Güvenior.Domain.Enums;

namespace Güvenior.Application.DTOs.FinancialGoal;

public class FinancialGoalDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public FinancialGoalType Type { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal CurrentSavings { get; set; }
    public decimal MonthlyContribution { get; set; }
    public decimal AnnualInflationRate { get; set; }
    public DateTime TargetDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
}
