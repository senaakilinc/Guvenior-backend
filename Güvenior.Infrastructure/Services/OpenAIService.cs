using Güvenior.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace Güvenior.Infrastructure.Services;

public class OpenAIService : IOpenAIService
{
    private readonly ChatClient _chatClient;

    public OpenAIService(IConfiguration configuration)
    {
        var apiKey = configuration["OpenAI:ApiKey"];
        _chatClient = new ChatClient("gpt-4o", apiKey);
    }

    public async Task<string> GenerateCoachingMessageAsync(string behavioralSummary)
    {
        var messages = new List<ChatMessage>
        {
            ChatMessage.CreateSystemMessage(
                "Sen Güvenior adlı bir finansal koçluk uygulamasının yapay zeka asistanısın. " +
                "Kullanıcılara yargılamadan, destekleyici ve samimi bir dille mesajlar üretiyorsun. " +
                "Yasaklayıcı dil kullanma, öneri sun. Kısa ve anlaşılır yaz, maksimum 3 cümle."
            ),
            ChatMessage.CreateUserMessage(behavioralSummary)
        };

        var response = await _chatClient.CompleteChatAsync(messages);
        return response.Value.Content[0].Text;
    }
}