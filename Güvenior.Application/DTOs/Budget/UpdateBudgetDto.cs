using Güvenior.Domain.Enums;

namespace Güvenior.Application.DTOs.Budget;

public class UpdateBudgetDto
{
    public ExpenseCategory? Category { get; set; }
    public decimal? LimitAmount { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
}
