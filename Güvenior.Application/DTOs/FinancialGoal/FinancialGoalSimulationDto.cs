namespace Güvenior.Application.DTOs.FinancialGoal;

public class FinancialGoalSimulationDto
{
    public int GoalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public FinancialGoalSimulationAnalysisDto Analysis { get; set; } = new();
    public string RuleBasedRecommendation { get; set; } = string.Empty;
    public string AiMessage { get; set; } = string.Empty;
}
