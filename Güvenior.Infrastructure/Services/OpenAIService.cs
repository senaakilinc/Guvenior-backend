using Güvenior.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace Güvenior.Infrastructure.Services;

public class OpenAIService : IOpenAIService
{
    private readonly ChatClient? _chatClient;
    private readonly ILogger<OpenAIService> _logger;

    public OpenAIService(IConfiguration configuration, ILogger<OpenAIService> logger)
    {
        _logger = logger;
        var apiKey = configuration["OpenAI:ApiKey"]?.Trim();
        var model = configuration["OpenAI:Model"] ?? "gpt-4o-mini";

        if (!string.IsNullOrWhiteSpace(apiKey))
            _chatClient = new ChatClient(model, apiKey);
        else
            _logger.LogWarning("OpenAI:ApiKey bulunamadi. AI mesajlari bos donecek.");
    }

    public async Task<string> GenerateCoachingMessageAsync(string behavioralSummary)
    {
        if (_chatClient == null)
            return string.Empty;

        var messages = new List<ChatMessage>
        {
            ChatMessage.CreateSystemMessage(
                "Sen Guvenior adli bir finansal kocluk uygulamasinin yapay zeka asistanisin. " +
                "Kullanicilara yargilamadan, destekleyici ve samimi bir dille mesajlar uretiyorsun. " +
                "Yasaklayici dil kullanma, oneri sun. Kisa ve anlasilir yaz, maksimum 3 cumle."
            ),
            ChatMessage.CreateUserMessage(behavioralSummary)
        };

        try
        {
            var response = await _chatClient.CompleteChatAsync(messages);
            return response.Value.Content[0].Text;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OpenAI mesaj uretimi basarisiz oldu.");
            return string.Empty;
        }
    }
}
