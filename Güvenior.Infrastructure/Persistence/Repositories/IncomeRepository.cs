using Güvenior.Application.Common.Interfaces;
using Güvenior.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Güvenior.Infrastructure.Persistence.Repositories;

public class IncomeRepository : IIncomeRepository
{
    private readonly ApplicationDbContext _context;

    public IncomeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Income income)
    {
        await _context.Incomes.AddAsync(income);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Income>> GetByUserIdAsync(string userId)
    {
        return await _context.Incomes
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.ReceivedDate)
            .ToListAsync();
    }
}