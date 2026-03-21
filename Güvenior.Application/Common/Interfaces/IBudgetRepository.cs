using Güvenior.Domain.Entities;

namespace Güvenior.Application.Common.Interfaces;

public interface IBudgetRepository
{
    Task AddAsync(Budget budget);
    Task<List<Budget>> GetByUserIdAsync(string userId);
}