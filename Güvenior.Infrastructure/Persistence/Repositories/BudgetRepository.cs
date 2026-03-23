using Güvenior.Application.Common.Interfaces;
using Güvenior.Domain.Entities;
using Güvenior.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Güvenior.Infrastructure.Persistence.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly ApplicationDbContext _context;

    public BudgetRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Budget budget)
    {
        await _context.Budgets.AddAsync(budget);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Budget>> GetByUserIdAsync(string userId)
    {
        return await _context.Budgets
            .Where(b => b.UserId == userId)
            .ToListAsync();
    }
    public async Task<Budget?> GetByUserCategoryAndMonthAsync(string userId, ExpenseCategory category, int month, int year)
    {
        return await _context.Budgets
            .FirstOrDefaultAsync(b =>
                b.UserId == userId &&
                b.Category == category &&
                b.Month == month &&
                b.Year == year);
    }
    public async Task UpdateAsync(Budget budget)
    {
        _context.Budgets.Update(budget);
        await _context.SaveChangesAsync();
    }
}