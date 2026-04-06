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
}