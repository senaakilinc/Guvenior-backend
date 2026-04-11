using Güvenior.Domain.Entities;

namespace Güvenior.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(User user, string encodedToken);
}
