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

    public async Task<BudgetEntity?> UpdateAsync(int id, string userId, UpdateBudgetDto dto)
    {
        var budget = await _budgetRepository.GetByIdAsync(id);
        if (budget == null || budget.UserId != userId)
            return null;

        var nextCategory = dto.Category ?? budget.Category;
        var nextMonth = dto.Month ?? budget.Month;
        var nextYear = dto.Year ?? budget.Year;

        var existing = await _budgetRepository.GetByUserCategoryAndMonthAsync(userId, nextCategory, nextMonth, nextYear);
        if (existing != null && existing.Id != budget.Id)
            throw new InvalidOperationException("Bu kategori ve ay için zaten bir bütçe mevcut.");

        budget.Category = nextCategory;

        if (dto.LimitAmount.HasValue)
            budget.LimitAmount = dto.LimitAmount.Value;

        budget.Month = nextMonth;
        budget.Year = nextYear;

        await _budgetRepository.UpdateAsync(budget);
        return budget;
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        var budget = await _budgetRepository.GetByIdAsync(id);
        if (budget == null || budget.UserId != userId)
            return false;

        await _budgetRepository.DeleteAsync(budget);
        return true;
    }
}
