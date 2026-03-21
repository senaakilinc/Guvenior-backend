using Güvenior.Domain.Enums;

namespace Güvenior.Application.DTOs.Income;

public class CreateIncomeDto
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime ReceivedDate { get; set; }
    public IncomeType Type { get; set; }
}