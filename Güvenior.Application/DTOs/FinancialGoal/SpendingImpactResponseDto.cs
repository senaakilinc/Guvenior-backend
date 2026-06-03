namespace Güvenior.Application.DTOs.FinancialGoal;

public class SpendingImpactResponseDto
{
    public SpendingImpactAnalysisDto Analysis { get; set; } = new();
    public string RuleBasedRecommendation { get; set; } = string.Empty;
    public string AiMessage { get; set; } = string.Empty;
}
