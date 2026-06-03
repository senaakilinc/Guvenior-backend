using Güvenior.Domain.Enums;

namespace Güvenior.Application.Common.Helpers;

public static class DisplayNameHelper
{
    public static string ToDisplayName(ExpenseCategory category)
    {
        return category switch
        {
            ExpenseCategory.Food => "Yemek",
            ExpenseCategory.Transport => "Ulasim",
            ExpenseCategory.Rent => "Kira",
            ExpenseCategory.Shopping => "Alisveris",
            ExpenseCategory.Entertainment => "Eglence",
            ExpenseCategory.Bills => "Faturalar",
            ExpenseCategory.Education => "Egitim",
            _ => "Diger"
        };
    }

    public static string ToDisplayName(FinancialGoalType type)
    {
        return type switch
        {
            FinancialGoalType.Home => "Ev",
            FinancialGoalType.Car => "Araba",
            FinancialGoalType.Rent => "Kira",
            FinancialGoalType.Education => "Egitim",
            FinancialGoalType.EmergencyFund => "Acil Durum Fonu",
            _ => "Diger"
        };
    }
}
