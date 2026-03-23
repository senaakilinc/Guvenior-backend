using Güvenior.Application.Common.Interfaces;
using Güvenior.Domain.Entities;
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
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .ToListAsync();
    }
}