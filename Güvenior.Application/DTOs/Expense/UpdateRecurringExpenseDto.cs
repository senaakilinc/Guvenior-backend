using Güvenior.Domain.Enums;

namespace Güvenior.Application.DTOs.Expense;

public class UpdateRecurringExpenseDto
{
    public string? Title { get; set; }
    public decimal? Amount { get; set; }
    public ExpenseCategory? Category { get; set; }
    public int? DayOfMonth { get; set; }
    public bool? IsActive { get; set; }
}
