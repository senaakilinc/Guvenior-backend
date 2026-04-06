using Güvenior.Domain.Common;
using Güvenior.Domain.Enums;

namespace Güvenior.Domain.Entities;

public class Budget : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public ExpenseCategory Category { get; set; }
    public decimal LimitAmount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}