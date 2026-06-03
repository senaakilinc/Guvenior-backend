namespace Güvenior.Application.DTOs.MonthlyReport;

public class MonthlyReportDto
{
    public MonthlyReportAnalysisDto Analysis { get; set; } = new();
    public string RuleBasedSummary { get; set; } = string.Empty;
    public string AiSummary { get; set; } = string.Empty;
}
