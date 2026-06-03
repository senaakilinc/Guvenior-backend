namespace Güvenior.Application.DTOs.FinancialGoal;

public class FinancialGoalSimulationAnalysisDto
{
    public int MonthsRemaining { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal ProjectedTargetPrice { get; set; }
    public decimal CurrentSavings { get; set; }
    public decimal PlannedTotalSavings { get; set; }
    public decimal FundingGap { get; set; }
    public decimal RequiredMonthlyContribution { get; set; }
    public decimal MonthlyContributionDifference { get; set; }
    public bool IsReachableWithCurrentPlan { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public decimal CurrentMonthIncome { get; set; }
    public decimal CurrentMonthExpense { get; set; }
    public decimal CurrentMonthSavingsCapacity { get; set; }
    public string? SuggestedCutCategory { get; set; }
    public decimal SuggestedMonthlyExpenseReduction { get; set; }
    public decimal AdjustedFundingGap { get; set; }
}
