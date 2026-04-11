using Güvenior.Domain.Enums;

namespace Güvenior.Application.DTOs.Expense;

public class CreateRecurringExpenseDto
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public ExpenseCategory Category { get; set; }
    public int DayOfMonth { get; set; }
}
