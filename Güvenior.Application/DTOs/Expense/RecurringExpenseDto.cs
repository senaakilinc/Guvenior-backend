using Güvenior.Domain.Enums;

namespace Güvenior.Application.DTOs.Expense;

public class RecurringExpenseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public ExpenseCategory Category { get; set; }
    public int DayOfMonth { get; set; }
    public bool IsActive { get; set; }
    public int? LastGeneratedYear { get; set; }
    public int? LastGeneratedMonth { get; set; }
}
