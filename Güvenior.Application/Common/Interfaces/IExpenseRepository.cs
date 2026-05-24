using Güvenior.Domain.Entities;

namespace Güvenior.Application.Common.Interfaces;

public interface IExpenseRepository
{
    Task AddAsync(Expense expense);
    Task<List<Expense>> GetByUserIdAsync(string userId);
    Task<Expense?> GetByIdAsync(int id);
    Task UpdateAsync(Expense expense);
    Task DeleteAsync(Expense expense);
}
