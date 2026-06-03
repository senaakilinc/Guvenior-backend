using Güvenior.Domain.Enums;

namespace Güvenior.Application.DTOs.Insight;

public class BehaviorAnalysisDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal SavingsAmount { get; set; }
    public decimal SavingsRate { get; set; }
    public decimal SalaryFirst48HourSpendingRate { get; set; }
    public decimal NightSpendingRate { get; set; }
    public decimal MonthlyExpenseIncreaseRate { get; set; }
    public ExpenseCategory? HighestBudgetUsageCategory { get; set; }
    public decimal HighestBudgetUsageRate { get; set; }
}
