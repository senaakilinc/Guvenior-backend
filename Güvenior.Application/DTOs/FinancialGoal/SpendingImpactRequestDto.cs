using Güvenior.Domain.Enums;

namespace Güvenior.Application.DTOs.FinancialGoal;

public class SpendingImpactRequestDto
{
    public int GoalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public ExpenseCategory Category { get; set; }
}
