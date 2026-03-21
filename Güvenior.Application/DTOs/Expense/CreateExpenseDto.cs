using Güvenior.Domain.Enums;

namespace Güvenior.Application.DTOs.Expense;

public class CreateExpenseDto
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public ExpenseCategory Category { get; set; }
    public DateTime SpentAt { get; set; }
}