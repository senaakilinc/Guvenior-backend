using Güvenior.Domain.Entities;

namespace Güvenior.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}