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
}