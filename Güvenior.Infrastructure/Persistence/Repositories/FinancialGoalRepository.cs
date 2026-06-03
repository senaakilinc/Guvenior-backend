using Güvenior.Application.Common.Interfaces;
using Güvenior.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Güvenior.Infrastructure.Persistence.Repositories;

public class FinancialGoalRepository : IFinancialGoalRepository
{
    private readonly ApplicationDbContext _context;

    public FinancialGoalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(FinancialGoal goal)
    {
        await _context.FinancialGoals.AddAsync(goal);
        await _context.SaveChangesAsync();
    }

    public async Task<List<FinancialGoal>> GetByUserIdAsync(string userId)
    {
        return await _context.FinancialGoals
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.TargetDate)
            .ToListAsync();
    }

    public async Task<FinancialGoal?> GetByIdAsync(int id)
    {
        return await _context.FinancialGoals.FindAsync(id);
    }

    public async Task UpdateAsync(FinancialGoal goal)
    {
        _context.FinancialGoals.Update(goal);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(FinancialGoal goal)
    {
        _context.FinancialGoals.Remove(goal);
        await _context.SaveChangesAsync();
    }
}
