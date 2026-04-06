using Güvenior.Domain.Enums;

namespace Güvenior.Application.DTOs.Budget;

public class CreateBudgetDto
{
    public ExpenseCategory Category { get; set; }
    public decimal LimitAmount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}