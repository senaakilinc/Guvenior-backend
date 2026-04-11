using Güvenior.Application.Common.Interfaces;
using Güvenior.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Güvenior.Infrastructure.Persistence.Repositories;

public class RecurringExpenseRepository : IRecurringExpenseRepository
{
    private readonly ApplicationDbContext _context;

    public RecurringExpenseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RecurringExpense>> GetAllByUserIdAsync(string userId)
    {
        return await _context.RecurringExpenses
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<RecurringExpense?> GetByIdAsync(int id)
    {
        return await _context.RecurringExpenses.FindAsync(id);
    }

    public async Task<RecurringExpense> AddAsync(RecurringExpense recurringExpense)
    {
        _context.RecurringExpenses.Add(recurringExpense);
        await _context.SaveChangesAsync();
        return recurringExpense;
    }

    public async Task UpdateAsync(RecurringExpense recurringExpense)
    {
        _context.RecurringExpenses.Update(recurringExpense);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(RecurringExpense recurringExpense)
    {
        _context.RecurringExpenses.Remove(recurringExpense);
        await _context.SaveChangesAsync();
    }
}
