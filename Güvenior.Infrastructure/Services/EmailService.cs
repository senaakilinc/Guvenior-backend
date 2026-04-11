using System.Net;
using System.Net.Mail;
using Güvenior.Application.Common.Interfaces;
using Güvenior.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Güvenior.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendPasswordResetEmailAsync(User user, string encodedToken)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
            throw new Exception("User email not found.");

        var frontendResetUrl = _configuration["Frontend:ResetPasswordUrl"]
            ?? throw new Exception("Frontend:ResetPasswordUrl is missing.");
        var smtpHost = _configuration["Smtp:Host"]
            ?? throw new Exception("Smtp:Host is missing.");
        var smtpUsername = _configuration["Smtp:Username"]
            ?? throw new Exception("Smtp:Username is missing.");
        var smtpPassword = _configuration["Smtp:Password"]
            ?? throw new Exception("Smtp:Password is missing.");
        var fromEmail = _configuration["Smtp:From"]
            ?? throw new Exception("Smtp:From is missing.");

        if (!int.TryParse(_configuration["Smtp:Port"], out var smtpPort))
            throw new Exception("Smtp:Port is invalid.");

        var enableSsl = bool.TryParse(_configuration["Smtp:EnableSsl"], out var parsedEnableSsl)
            ? parsedEnableSsl
            : true;

        var resetLink =
            $"{frontendResetUrl}?email={Uri.EscapeDataString(user.Email)}&token={encodedToken}";

        var body = $@"
<p>Hello {user.FullName},</p>
<p>Click the link below to reset your password:</p>
<p><a href=""{resetLink}"">Reset password</a></p>
<p>If you did not request this, you can ignore this email.</p>";

        using var message = new MailMessage(fromEmail, user.Email)
        {
            Subject = "Password reset",
            Body = body,
            IsBodyHtml = true
        };

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUsername, smtpPassword),
            EnableSsl = enableSsl
        };

        await client.SendMailAsync(message);
    }
}
