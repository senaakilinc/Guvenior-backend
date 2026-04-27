using Güvenior.Domain.Common;
using Güvenior.Domain.Enums;

namespace Güvenior.Domain.Entities;

public class RecurringExpense : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public ExpenseCategory Category { get; set; }
    public int DayOfMonth { get; set; }
    public bool IsActive { get; set; } = true;

    // Used to prevent generating the same recurring expense multiple times in the same month.
    public int? LastGeneratedYear { get; set; }
    public int? LastGeneratedMonth { get; set; }
}
