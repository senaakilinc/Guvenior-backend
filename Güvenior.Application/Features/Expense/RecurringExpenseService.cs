using Güvenior.Application.Common.Interfaces;
using Güvenior.Application.DTOs.Expense;
using Güvenior.Domain.Entities;
using ExpenseEntity = Güvenior.Domain.Entities.Expense;

namespace Güvenior.Application.Features.Expense;

public class RecurringExpenseService
{
    private readonly IRecurringExpenseRepository _repository;
    private readonly IExpenseRepository _expenseRepository;

    public RecurringExpenseService(
        IRecurringExpenseRepository repository,
        IExpenseRepository expenseRepository)
    {
        _repository = repository;
        _expenseRepository = expenseRepository;
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
            IsActive = e.IsActive,
            LastGeneratedYear = e.LastGeneratedYear,
            LastGeneratedMonth = e.LastGeneratedMonth,
        });
    }

    public async Task<RecurringExpenseDto> GetByIdAsync(int id)
    {
        var expense = await _repository.GetByIdAsync(id);
        if (expense == null) throw new Exception("TapÄ±lamadÄ±");

        return new RecurringExpenseDto
        {
            Id = expense.Id,
            Title = expense.Title,
            Amount = expense.Amount,
            Category = expense.Category,
            DayOfMonth = expense.DayOfMonth,
            IsActive = expense.IsActive,
            LastGeneratedYear = expense.LastGeneratedYear,
            LastGeneratedMonth = expense.LastGeneratedMonth,
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
            IsActive = expense.IsActive,
            LastGeneratedYear = expense.LastGeneratedYear,
            LastGeneratedMonth = expense.LastGeneratedMonth,
        };
    }

    public async Task<RecurringExpenseDto?> UpdateAsync(int id, string userId, UpdateRecurringExpenseDto dto)
    {
        var expense = await _repository.GetByIdAsync(id);
        if (expense == null || expense.UserId != userId)
            return null;

        if (!string.IsNullOrWhiteSpace(dto.Title))
            expense.Title = dto.Title;

        if (dto.Amount.HasValue)
            expense.Amount = dto.Amount.Value;

        if (dto.Category.HasValue)
            expense.Category = dto.Category.Value;

        if (dto.DayOfMonth.HasValue)
            expense.DayOfMonth = dto.DayOfMonth.Value;

        if (dto.IsActive.HasValue)
            expense.IsActive = dto.IsActive.Value;

        await _repository.UpdateAsync(expense);

        return new RecurringExpenseDto
        {
            Id = expense.Id,
            Title = expense.Title,
            Amount = expense.Amount,
            Category = expense.Category,
            DayOfMonth = expense.DayOfMonth,
            IsActive = expense.IsActive,
            LastGeneratedYear = expense.LastGeneratedYear,
            LastGeneratedMonth = expense.LastGeneratedMonth,
        };
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        var expense = await _repository.GetByIdAsync(id);
        if (expense == null || expense.UserId != userId)
            return false;

        await _repository.DeleteAsync(expense);
        return true;
    }

    public async Task<int> GenerateDueExpensesAsync(string userId, DateTime utcNow)
    {
        utcNow = DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);

        var recurring = await _repository.GetAllByUserIdAsync(userId);
        var due = recurring
            .Where(r => r.IsActive)
            .Where(r =>
            {
                var scheduledDay = Math.Clamp(r.DayOfMonth, 1, DateTime.DaysInMonth(utcNow.Year, utcNow.Month));
                return utcNow.Day >= scheduledDay;
            })
            .ToList();

        var createdCount = 0;

        foreach (var r in due)
        {
            if (r.LastGeneratedYear == utcNow.Year && r.LastGeneratedMonth == utcNow.Month)
                continue;

            var scheduledDay = Math.Clamp(r.DayOfMonth, 1, DateTime.DaysInMonth(utcNow.Year, utcNow.Month));
            var spentAt = new DateTime(utcNow.Year, utcNow.Month, scheduledDay, 0, 0, 0, DateTimeKind.Utc);

            await _expenseRepository.AddAsync(new ExpenseEntity
            {
                UserId = userId,
                Title = r.Title,
                Amount = r.Amount,
                Category = r.Category,
                SpentAt = spentAt,
            });

            r.LastGeneratedYear = utcNow.Year;
            r.LastGeneratedMonth = utcNow.Month;
            await _repository.UpdateAsync(r);

            createdCount++;
        }

        return createdCount;
    }
}
