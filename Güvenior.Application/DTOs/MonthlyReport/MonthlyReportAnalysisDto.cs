namespace Güvenior.Application.DTOs.MonthlyReport;

public class MonthlyReportAnalysisDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal SavingsAmount { get; set; }
    public decimal SavingsRate { get; set; }
    public string? TopExpenseCategory { get; set; }
    public decimal TopExpenseCategoryAmount { get; set; }
    public decimal NightSpendingRate { get; set; }
    public decimal SalaryFirst48HourSpendingRate { get; set; }
    public decimal FlexibleExpenseTotal { get; set; }
    public int ActiveGoalCount { get; set; }
    public int HighRiskGoalCount { get; set; }
    public string BehaviorProfile { get; set; } = string.Empty;
}
