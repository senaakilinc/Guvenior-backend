using Güvenior.Domain.Enums;

namespace Güvenior.Application.DTOs.FinancialGoal;

public class SpendingImpactAnalysisDto
{
    public int GoalId { get; set; }
    public string GoalTitle { get; set; } = string.Empty;
    public string ExpenseTitle { get; set; } = string.Empty;
    public ExpenseCategory ExpenseCategory { get; set; }
    public string ExpenseCategoryName { get; set; } = string.Empty;
    public decimal ExpenseAmount { get; set; }
    public decimal OriginalFundingGap { get; set; }
    public decimal NewFundingGap { get; set; }
    public decimal ImpactAmount { get; set; }
    public int EstimatedDelayMonths { get; set; }
    public decimal SuggestedMonthlyOffset { get; set; }
    public string ImpactLevel { get; set; } = string.Empty;
}
