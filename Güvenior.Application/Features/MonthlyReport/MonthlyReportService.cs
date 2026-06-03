using Güvenior.Application.Common.Interfaces;
using Güvenior.Application.DTOs.MonthlyReport;
using Güvenior.Domain.Enums;

namespace Güvenior.Application.Features.MonthlyReport;

public class MonthlyReportService
{
    private readonly IIncomeRepository _incomeRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IFinancialGoalRepository _financialGoalRepository;
    private readonly IOpenAIService _openAIService;

    public MonthlyReportService(
        IIncomeRepository incomeRepository,
        IExpenseRepository expenseRepository,
        IFinancialGoalRepository financialGoalRepository,
        IOpenAIService openAIService)
    {
        _incomeRepository = incomeRepository;
        _expenseRepository = expenseRepository;
        _financialGoalRepository = financialGoalRepository;
        _openAIService = openAIService;
    }

    public async Task<MonthlyReportDto> GenerateAsync(string userId, int? month = null, int? year = null)
    {
        var now = DateTime.UtcNow;
        var reportMonth = month ?? now.Month;
        var reportYear = year ?? now.Year;
        var analysis = await BuildAnalysisAsync(userId, reportMonth, reportYear);
        var ruleBasedSummary = BuildRuleBasedSummary(analysis);
        var aiSummary = await BuildAiSummaryAsync(analysis, ruleBasedSummary);

        return new MonthlyReportDto
        {
            Analysis = analysis,
            RuleBasedSummary = ruleBasedSummary,
            AiSummary = aiSummary
        };
    }

    private async Task<MonthlyReportAnalysisDto> BuildAnalysisAsync(string userId, int month, int year)
    {
        var incomes = await _incomeRepository.GetByUserIdAsync(userId);
        var expenses = await _expenseRepository.GetByUserIdAsync(userId);
        var goals = await _financialGoalRepository.GetByUserIdAsync(userId);

        var monthlyIncomes = incomes
            .Where(x => x.ReceivedDate.Month == month && x.ReceivedDate.Year == year)
            .ToList();
        var monthlyExpenses = expenses
            .Where(x => x.SpentAt.Month == month && x.SpentAt.Year == year)
            .ToList();

        var totalIncome = monthlyIncomes.Sum(x => x.Amount);
        var totalExpense = monthlyExpenses.Sum(x => x.Amount);
        var topCategory = monthlyExpenses
            .GroupBy(x => x.Category)
            .Select(x => new
            {
                Category = x.Key,
                Amount = x.Sum(expense => expense.Amount)
            })
            .OrderByDescending(x => x.Amount)
            .FirstOrDefault();

        var nightExpense = monthlyExpenses
            .Where(x => x.SpentAt.Hour >= 23 || x.SpentAt.Hour < 5)
            .Sum(x => x.Amount);

        var latestIncome = monthlyIncomes.OrderByDescending(x => x.ReceivedDate).FirstOrDefault();
        var salaryFirst48HourExpense = latestIncome == null
            ? 0
            : monthlyExpenses
                .Where(x => x.SpentAt >= latestIncome.ReceivedDate && x.SpentAt <= latestIncome.ReceivedDate.AddHours(48))
                .Sum(x => x.Amount);

        var flexibleCategories = new[]
        {
            ExpenseCategory.Shopping,
            ExpenseCategory.Entertainment,
            ExpenseCategory.Food,
            ExpenseCategory.Transport,
            ExpenseCategory.Other
        };
        var flexibleExpenseTotal = monthlyExpenses
            .Where(x => flexibleCategories.Contains(x.Category))
            .Sum(x => x.Amount);

        var activeGoals = goals.Where(x => !x.IsCompleted).ToList();
        var highRiskGoalCount = activeGoals.Count(IsHighRiskGoal);

        var analysis = new MonthlyReportAnalysisDto
        {
            Month = month,
            Year = year,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            SavingsAmount = totalIncome - totalExpense,
            SavingsRate = Divide(totalIncome - totalExpense, totalIncome),
            TopExpenseCategory = topCategory == null
                ? null
                : Güvenior.Application.Common.Helpers.DisplayNameHelper.ToDisplayName(topCategory.Category),
            TopExpenseCategoryAmount = topCategory?.Amount ?? 0,
            NightSpendingRate = Divide(nightExpense, totalExpense),
            SalaryFirst48HourSpendingRate = Divide(salaryFirst48HourExpense, latestIncome?.Amount ?? 0),
            FlexibleExpenseTotal = flexibleExpenseTotal,
            ActiveGoalCount = activeGoals.Count,
            HighRiskGoalCount = highRiskGoalCount
        };

        analysis.BehaviorProfile = BuildBehaviorProfile(analysis);
        return analysis;
    }

    private static bool IsHighRiskGoal(Güvenior.Domain.Entities.FinancialGoal goal)
    {
        var today = DateTime.UtcNow.Date;
        var monthsRemaining = Math.Max(1, ((goal.TargetDate.Year - today.Year) * 12) + goal.TargetDate.Month - today.Month);
        var yearsRemaining = monthsRemaining / 12m;
        var inflationMultiplier = (decimal)Math.Pow((double)(1 + (goal.AnnualInflationRate / 100m)), (double)yearsRemaining);
        var projectedTargetPrice = goal.CurrentPrice * inflationMultiplier;
        var plannedTotalSavings = goal.CurrentSavings + (goal.MonthlyContribution * monthsRemaining);
        var coverageRate = projectedTargetPrice <= 0 ? 1 : plannedTotalSavings / projectedTargetPrice;
        return coverageRate < 0.75m;
    }

    private static string BuildBehaviorProfile(MonthlyReportAnalysisDto analysis)
    {
        if (analysis.TotalIncome <= 0 && analysis.TotalExpense <= 0)
            return "Veri Bekleniyor";

        if (analysis.SalaryFirst48HourSpendingRate >= 0.35m)
            return "Maas Sonrasi Hizlanan";

        if (analysis.NightSpendingRate >= 0.15m)
            return "Gece Harcamasina Acik";

        if (analysis.SavingsRate >= 0.20m)
            return "Hedef Odakli";

        if (analysis.SavingsRate < 0.10m)
            return "Birikim Payi Dusuk";

        return "Dengeli Ilerleyen";
    }

    private static string BuildRuleBasedSummary(MonthlyReportAnalysisDto analysis)
    {
        if (analysis.TotalIncome <= 0 && analysis.TotalExpense <= 0)
            return "Bu ay icin henuz yeterli veri yok. Gelir ve harcama kayitlari eklendikce aylik finansal ozet daha anlamli hale gelir.";

        var topCategory = string.IsNullOrWhiteSpace(analysis.TopExpenseCategory)
            ? "belirgin bir kategori"
            : analysis.TopExpenseCategory;

        return $"Bu ay {analysis.TotalIncome:0.##} TL gelir ve {analysis.TotalExpense:0.##} TL harcama kaydedildi. En yogun kategori {topCategory}; birikim orani {analysis.SavingsRate * 100:0.#}% seviyesinde. Davranis profili: {analysis.BehaviorProfile}.";
    }

    private async Task<string> BuildAiSummaryAsync(MonthlyReportAnalysisDto analysis, string ruleBasedSummary)
    {
        var summary =
            $"Ay: {analysis.Month}/{analysis.Year}\n" +
            $"Toplam gelir: {analysis.TotalIncome:0.##} TL\n" +
            $"Toplam harcama: {analysis.TotalExpense:0.##} TL\n" +
            $"Birikim tutari: {analysis.SavingsAmount:0.##} TL\n" +
            $"Birikim orani: {analysis.SavingsRate * 100:0.#}%\n" +
            $"En yogun kategori: {analysis.TopExpenseCategory ?? "Yok"} ({analysis.TopExpenseCategoryAmount:0.##} TL)\n" +
            $"Gece harcama orani: {analysis.NightSpendingRate * 100:0.#}%\n" +
            $"Maas sonrasi ilk 48 saat harcama orani: {analysis.SalaryFirst48HourSpendingRate * 100:0.#}%\n" +
            $"Esnek harcama toplami: {analysis.FlexibleExpenseTotal:0.##} TL\n" +
            $"Aktif hedef sayisi: {analysis.ActiveGoalCount}\n" +
            $"Yuksek riskli hedef sayisi: {analysis.HighRiskGoalCount}\n" +
            $"Davranis profili: {analysis.BehaviorProfile}\n" +
            $"Kural ozeti: {ruleBasedSummary}\n" +
            "Bu verilerle yeni mezun bir kullaniciya yargilamayan, destekleyici, en fazla 3 cumlelik aylik finans ozeti yaz. Sayilari degistirme veya yeni sayi uydurma.";

        var message = await _openAIService.GenerateCoachingMessageAsync(summary);
        return string.IsNullOrWhiteSpace(message) ? string.Empty : message;
    }

    private static decimal Divide(decimal value, decimal divisor)
    {
        return divisor == 0 ? 0 : Math.Round(value / divisor, 4);
    }
}
