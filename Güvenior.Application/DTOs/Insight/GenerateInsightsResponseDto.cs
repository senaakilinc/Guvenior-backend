namespace Güvenior.Application.DTOs.Insight;

public class GenerateInsightsResponseDto
{
    public BehaviorAnalysisDto Analysis { get; set; } = new();
    public List<InsightDto> Insights { get; set; } = new();
}
