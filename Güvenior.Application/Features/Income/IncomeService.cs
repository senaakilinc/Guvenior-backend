using Güvenior.Application.Common.Interfaces;
using Güvenior.Application.DTOs.Income;
using IncomeEntity = Güvenior.Domain.Entities.Income;

namespace Güvenior.Application.Features.Income;

public class IncomeService
{
    private readonly IIncomeRepository _incomeRepository;

    public IncomeService(IIncomeRepository incomeRepository)
    {
        _incomeRepository = incomeRepository;
    }

    public async Task AddAsync(string userId, CreateIncomeDto dto)
    {
        var income = new IncomeEntity
        {
            UserId = userId,
            Title = dto.Title,
            Amount = dto.Amount,
            ReceivedDate = DateTime.SpecifyKind(dto.ReceivedDate, DateTimeKind.Utc),
            Type = dto.Type
        };

        await _incomeRepository.AddAsync(income);
    }

    public async Task<List<IncomeEntity>> GetByUserIdAsync(string userId)
    {
        return await _incomeRepository.GetByUserIdAsync(userId);
    }

    public async Task<IncomeEntity?> UpdateAsync(int id, string userId, UpdateIncomeDto dto)
    {
        var income = await _incomeRepository.GetByIdAsync(id);
        if (income == null || income.UserId != userId)
            return null;

        if (!string.IsNullOrWhiteSpace(dto.Title))
            income.Title = dto.Title;

        if (dto.Amount.HasValue)
            income.Amount = dto.Amount.Value;

        if (dto.ReceivedDate.HasValue)
            income.ReceivedDate = DateTime.SpecifyKind(dto.ReceivedDate.Value, DateTimeKind.Utc);

        if (dto.Type.HasValue)
            income.Type = dto.Type.Value;

        await _incomeRepository.UpdateAsync(income);
        return income;
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        var income = await _incomeRepository.GetByIdAsync(id);
        if (income == null || income.UserId != userId)
            return false;

        await _incomeRepository.DeleteAsync(income);
        return true;
    }
}
