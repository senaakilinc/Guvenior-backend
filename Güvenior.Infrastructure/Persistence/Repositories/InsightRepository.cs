using Güvenior.Application.Common.Interfaces;
using Güvenior.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Güvenior.Infrastructure.Persistence.Repositories;

public class InsightRepository : IInsightRepository
{
    private readonly ApplicationDbContext _context;

    public InsightRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Insight insight)
    {
        await _context.Insights.AddAsync(insight);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Insight>> GetByUserIdAsync(string userId)
    {
        return await _context.Insights
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
}