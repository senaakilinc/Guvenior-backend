using Güvenior.Application.Common.Interfaces;
using Güvenior.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Güvenior.Infrastructure.Persistence.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly ApplicationDbContext _context;

    public ExpenseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Expense expense)
    {
        await _context.Expenses.AddAsync(expense);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Expense>> GetByUserIdAsync(string userId)
    {
        return await _context.Expenses
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.SpentAt)
            .ToListAsync();
    }
}