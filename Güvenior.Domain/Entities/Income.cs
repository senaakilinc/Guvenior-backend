using Güvenior.Domain.Common;
using Güvenior.Domain.Enums;

namespace Güvenior.Domain.Entities;

public class Income : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime ReceivedDate { get; set; }
    public IncomeType Type { get; set; }
}