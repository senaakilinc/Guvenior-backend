using Güvenior.Application.Common.Interfaces;
using Güvenior.Domain.Enums;

namespace Güvenior.Infrastructure.Services;

public class MarketAssumptionService : IMarketAssumptionService
{
    public decimal GetDefaultAnnualInflationRate(FinancialGoalType goalType)
    {
        return goalType switch
        {
            FinancialGoalType.Home => 45m,
            FinancialGoalType.Car => 35m,
            FinancialGoalType.Rent => 50m,
            FinancialGoalType.Education => 30m,
            FinancialGoalType.EmergencyFund => 30m,
            _ => 35m
        };
    }
}
