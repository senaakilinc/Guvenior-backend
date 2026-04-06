using Güvenior.Application.Common.Interfaces;
using Güvenior.Application.DTOs.Expense;
using Güvenior.Domain.Entities;

namespace Güvenior.Application.Features.Expense;

public class RecurringExpenseService
{
    private readonly IRecurringExpenseRepository _repository;

    public RecurringExpenseService(IRecurringExpenseRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<RecurringExpenseDto>> GetAllAsync(string userId)
    {
        var expenses = await _repository.GetAllByUserIdAsync(userId);
        return expenses.Select(e => new RecurringExpenseDto
        {
            Id = e.Id,
            Title = e.Title,
            Amount = e.Amount,
            Category = e.Category,
            DayOfMonth = e.DayOfMonth,
            IsActive = e.IsActive
        });
    }

    public async Task<RecurringExpenseDto> GetByIdAsync(int id)
    {
        var expense = await _repository.GetByIdAsync(id);
        if (expense == null) throw new Exception("Tapılamadı");

        return new RecurringExpenseDto
        {
            Id = expense.Id,
            Title = expense.Title,
            Amount = expense.Amount,
            Category = expense.Category,
            DayOfMonth = expense.DayOfMonth,
            IsActive = expense.IsActive
        };
    }

    public async Task<RecurringExpenseDto> CreateAsync(string userId, CreateRecurringExpenseDto dto)
    {
        var expense = new RecurringExpense
        {
            UserId = userId,
            Title = dto.Title,
            Amount = dto.Amount,
            Category = dto.Category,
            DayOfMonth = dto.DayOfMonth,
            IsActive = true
        };

        await _repository.AddAsync(expense);

        return new RecurringExpenseDto
        {
            Id = expense.Id,
            Title = expense.Title,
            Amount = expense.Amount,
            Category = expense.Category,
            DayOfMonth = expense.DayOfMonth,
            IsActive = expense.IsActive
        };
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var expense = await _repository.GetByIdAsync(id);
        if (expense == null || expense.UserId != userId)
            throw new Exception("Bulunamadı veya yetkisiz");

        await _repository.DeleteAsync(expense);
    }
}
