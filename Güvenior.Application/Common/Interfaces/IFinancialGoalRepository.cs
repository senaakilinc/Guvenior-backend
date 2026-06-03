using Güvenior.Domain.Entities;

namespace Güvenior.Application.Common.Interfaces;

public interface IFinancialGoalRepository
{
    Task AddAsync(FinancialGoal goal);
    Task<List<FinancialGoal>> GetByUserIdAsync(string userId);
    Task<FinancialGoal?> GetByIdAsync(int id);
    Task UpdateAsync(FinancialGoal goal);
    Task DeleteAsync(FinancialGoal goal);
}
