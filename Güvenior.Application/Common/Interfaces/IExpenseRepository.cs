using Güvenior.Domain.Entities;

namespace Güvenior.Application.Common.Interfaces;

public interface IExpenseRepository
{
    Task AddAsync(Expense expense);
    Task<List<Expense>> GetByUserIdAsync(string userId);
}