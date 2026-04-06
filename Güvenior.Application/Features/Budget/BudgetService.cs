using Güvenior.Application.Common.Interfaces;
using Güvenior.Application.DTOs.Budget;
using BudgetEntity = Güvenior.Domain.Entities.Budget;

namespace Güvenior.Application.Features.Budget;

public class BudgetService
{
    private readonly IBudgetRepository _budgetRepository;

    public BudgetService(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    public async Task AddOrUpdateAsync(string userId, CreateBudgetDto dto)
    {
        var existing = await _budgetRepository
            .GetByUserCategoryAndMonthAsync(userId, dto.Category, dto.Month, dto.Year);

        if (existing != null)
        {
            existing.LimitAmount = dto.LimitAmount;
            await _budgetRepository.UpdateAsync(existing);
        }
        else
        {
            var budget = new BudgetEntity
            {
                UserId = userId,
                Category = dto.Category,
                LimitAmount = dto.LimitAmount,
                Month = dto.Month,
                Year = dto.Year
            };
            await _budgetRepository.AddAsync(budget);
        }
    }

    public async Task<List<BudgetEntity>> GetByUserIdAsync(string userId)
    {
        return await _budgetRepository.GetByUserIdAsync(userId);
    }
}
