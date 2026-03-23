using Güvenior.Domain.Entities;
using Güvenior.Domain.Enums;

namespace Güvenior.Application.Common.Interfaces;

public interface IBudgetRepository
{
    Task AddAsync(Budget budget);
    Task<List<Budget>> GetByUserIdAsync(string userId);
    Task<Budget?> GetByUserCategoryAndMonthAsync(string userId, ExpenseCategory category, int month, int year);
    Task UpdateAsync(Budget budget);
}