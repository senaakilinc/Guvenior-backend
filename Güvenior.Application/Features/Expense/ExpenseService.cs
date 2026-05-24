using Güvenior.Application.Common.Interfaces;
using Güvenior.Application.DTOs.Expense;
using ExpenseEntity = Güvenior.Domain.Entities.Expense;

namespace Güvenior.Application.Features.Expense;

public class ExpenseService
{
    private readonly IExpenseRepository _expenseRepository;

    public ExpenseService(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task AddAsync(string userId, CreateExpenseDto dto)
    {
        var expense = new ExpenseEntity
        {
            UserId = userId,
            Title = dto.Title,
            Amount = dto.Amount,
            Category = dto.Category,
            SpentAt = DateTime.SpecifyKind(dto.SpentAt, DateTimeKind.Utc),
        };

        await _expenseRepository.AddAsync(expense);
    }

    public async Task<List<ExpenseEntity>> GetByUserIdAsync(string userId)
    {
        return await _expenseRepository.GetByUserIdAsync(userId);
    }

    public async Task<ExpenseEntity?> UpdateAsync(int id, string userId, UpdateExpenseDto dto)
    {
        var expense = await _expenseRepository.GetByIdAsync(id);
        if (expense == null || expense.UserId != userId)
            return null;

        if (!string.IsNullOrWhiteSpace(dto.Title))
            expense.Title = dto.Title;

        if (dto.Amount.HasValue)
            expense.Amount = dto.Amount.Value;

        if (dto.Category.HasValue)
            expense.Category = dto.Category.Value;

        if (dto.SpentAt.HasValue)
            expense.SpentAt = DateTime.SpecifyKind(dto.SpentAt.Value, DateTimeKind.Utc);

        await _expenseRepository.UpdateAsync(expense);
        return expense;
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        var expense = await _expenseRepository.GetByIdAsync(id);
        if (expense == null || expense.UserId != userId)
            return false;

        await _expenseRepository.DeleteAsync(expense);
        return true;
    }
}
