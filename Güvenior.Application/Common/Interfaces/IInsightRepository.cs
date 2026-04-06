using Güvenior.Domain.Entities;

namespace Güvenior.Application.Common.Interfaces;

public interface IInsightRepository
{
    Task AddAsync(Insight insight);
    Task<List<Insight>> GetByUserIdAsync(string userId);
}