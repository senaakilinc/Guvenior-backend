using Güvenior.Domain.Enums;

namespace Güvenior.Application.DTOs.Income;

public class UpdateIncomeDto
{
    public string? Title { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public IncomeType? Type { get; set; }
}
