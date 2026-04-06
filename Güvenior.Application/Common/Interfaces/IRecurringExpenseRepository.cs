using Güvenior.Domain.Entities;

namespace Güvenior.Application.Common.Interfaces;

public interface IRecurringExpenseRepository
{
    Task<IEnumerable<RecurringExpense>> GetAllByUserIdAsync(string userId);
    Task<RecurringExpense?> GetByIdAsync(int id);
    Task<RecurringExpense> AddAsync(RecurringExpense recurringExpense);
    Task DeleteAsync(RecurringExpense recurringExpense);
}
