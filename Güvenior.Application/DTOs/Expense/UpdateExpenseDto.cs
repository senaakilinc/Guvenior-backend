using Güvenior.Domain.Enums;

namespace Güvenior.Application.DTOs.Expense;

public class UpdateExpenseDto
{
    public string? Title { get; set; }
    public decimal? Amount { get; set; }
    public ExpenseCategory? Category { get; set; }
    public DateTime? SpentAt { get; set; }
}
