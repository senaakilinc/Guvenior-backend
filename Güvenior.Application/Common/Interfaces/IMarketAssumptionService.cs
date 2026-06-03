using Güvenior.Domain.Enums;

namespace Güvenior.Application.Common.Interfaces;

public interface IMarketAssumptionService
{
    decimal GetDefaultAnnualInflationRate(FinancialGoalType goalType);
}
