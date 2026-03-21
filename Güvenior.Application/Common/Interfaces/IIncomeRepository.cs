using Güvenior.Domain.Entities;

namespace Güvenior.Application.Common.Interfaces;

public interface IIncomeRepository
{
    Task AddAsync(Income income);
    Task<List<Income>> GetByUserIdAsync(string userId);
}