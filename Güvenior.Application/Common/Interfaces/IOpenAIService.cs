namespace Güvenior.Application.Common.Interfaces;

public interface IOpenAIService
{
    Task<string> GenerateCoachingMessageAsync(string behavioralSummary);
}