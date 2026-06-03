using System.Globalization;
using Güvenior.Application.Common.Interfaces;
using Güvenior.Application.DTOs.Insight;
using Güvenior.Domain.Entities;

namespace Güvenior.Application.Features.Insight;

public class InsightService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IIncomeRepository _incomeRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly IInsightRepository _insightRepository;
    private readonly IOpenAIService _openAIService;

    public InsightService(
        IExpenseRepository expenseRepository,
        IIncomeRepository incomeRepository,
        IBudgetRepository budgetRepository,
        IInsightRepository insightRepository,
        IOpenAIService openAIService)
    {
        _expenseRepository = expenseRepository;
        _incomeRepository = incomeRepository;
        _budgetRepository = budgetRepository;
        _insightRepository = insightRepository;
        _openAIService = openAIService;
    }

    public async Task<List<InsightDto>> GetByUserIdAsync(string userId)
    {
        var insights = await _insightRepository.GetByUserIdAsync(userId);
        return insights.Select(ToDto).ToList();
    }

    public async Task<GenerateInsightsResponseDto> GenerateForCurrentMonthAsync(string userId)
    {
        var now = DateTime.UtcNow;
        var analysis = await AnalyzeAsync(userId, now.Month, now.Year);
        var candidates = BuildInsightCandidates(analysis);
        var existingInsights = await _insightRepository.GetByUserIdAsync(userId);
        var currentMonthInsights = existingInsights
            .Where(x => x.CreatedAt.Month == now.Month && x.CreatedAt.Year == now.Year)
            .ToList();
        var createdInsights = new List<InsightDto>();

        foreach (var candidate in candidates)
        {
            var existing = currentMonthInsights
                .FirstOrDefault(x => x.Title == candidate.Title);
            if (existing != null)
            {
                createdInsights.Add(ToDto(existing));
                continue;
            }

            var message = await TryCreateCoachingMessageAsync(candidate.Title, candidate.Message, analysis);
            var insight = new Domain.Entities.Insight
            {
                UserId = userId,
                Title = candidate.Title,
                Message = message
            };

            await _insightRepository.AddAsync(insight);
            createdInsights.Add(ToDto(insight));
        }

        return new GenerateInsightsResponseDto
        {
            Analysis = analysis,
            Insights = createdInsights
        };
    }

    public async Task<bool> MarkAsReadAsync(int id, string userId)
    {
        var insight = await _insightRepository.GetByIdAsync(id);
        if (insight == null || insight.UserId != userId)
            return false;

        insight.IsRead = true;
        await _insightRepository.UpdateAsync(insight);
        return true;
    }

    private async Task<BehaviorAnalysisDto> AnalyzeAsync(string userId, int month, int year)
    {
        var expenses = await _expenseRepository.GetByUserIdAsync(userId);
        var incomes = await _incomeRepository.GetByUserIdAsync(userId);
        var budgets = await _budgetRepository.GetByUserIdAsync(userId);

        var currentExpenses = expenses
            .Where(x => x.SpentAt.Month == month && x.SpentAt.Year == year)
            .ToList();
        var currentIncomes = incomes
            .Where(x => x.ReceivedDate.Month == month && x.ReceivedDate.Year == year)
            .ToList();

        var previousMonth = new DateTime(year, month, 1).AddMonths(-1);
        var previousExpenses = expenses
            .Where(x => x.SpentAt.Month == previousMonth.Month && x.SpentAt.Year == previousMonth.Year)
            .ToList();

        var totalIncome = currentIncomes.Sum(x => x.Amount);
        var totalExpense = currentExpenses.Sum(x => x.Amount);
        var previousExpenseTotal = previousExpenses.Sum(x => x.Amount);

        var latestIncome = currentIncomes.OrderByDescending(x => x.ReceivedDate).FirstOrDefault();
        var salaryFirst48Expense = latestIncome == null
            ? 0
            : currentExpenses
                .Where(x => x.SpentAt >= latestIncome.ReceivedDate && x.SpentAt <= latestIncome.ReceivedDate.AddHours(48))
                .Sum(x => x.Amount);

        var nightExpense = currentExpenses
            .Where(x => x.SpentAt.Hour >= 23 || x.SpentAt.Hour < 5)
            .Sum(x => x.Amount);

        var currentBudgets = budgets
            .Where(x => x.Month == month && x.Year == year && x.LimitAmount > 0)
            .ToList();

        var budgetUsage = currentBudgets
            .Select(budget => new
            {
                budget.Category,
                UsageRate = currentExpenses
                    .Where(expense => expense.Category == budget.Category)
                    .Sum(expense => expense.Amount) / budget.LimitAmount
            })
            .OrderByDescending(x => x.UsageRate)
            .FirstOrDefault();

        return new BehaviorAnalysisDto
        {
            Month = month,
            Year = year,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            SavingsAmount = totalIncome - totalExpense,
            SavingsRate = Divide(totalIncome - totalExpense, totalIncome),
            SalaryFirst48HourSpendingRate = Divide(salaryFirst48Expense, latestIncome?.Amount ?? 0),
            NightSpendingRate = Divide(nightExpense, totalExpense),
            MonthlyExpenseIncreaseRate = previousExpenseTotal <= 0 ? 0 : Divide(totalExpense - previousExpenseTotal, previousExpenseTotal),
            HighestBudgetUsageCategory = budgetUsage?.Category,
            HighestBudgetUsageRate = budgetUsage?.UsageRate ?? 0
        };
    }

    private static List<(string Title, string Message)> BuildInsightCandidates(BehaviorAnalysisDto analysis)
    {
        var insights = new List<(string Title, string Message)>();

        if (analysis.TotalIncome <= 0 && analysis.TotalExpense <= 0)
        {
            insights.Add(("Veri bekleniyor", "Bu ay icin henuz yeterli gelir ve harcama verisi yok. Bir iki kayit ekledikten sonra daha anlamli oneriler sunabiliriz."));
            return insights;
        }

        if (analysis.SalaryFirst48HourSpendingRate >= 0.35m)
            insights.Add(("Maas sonrasi tempo yuksek", "Maas sonrasi ilk 48 saatte harcama orani yuksek gorunuyor. Ilk iki gun icin kucuk bir bekleme kurali hedeflerini korumana yardim edebilir."));

        if (analysis.NightSpendingRate >= 0.15m)
            insights.Add(("Gece harcamalari dikkat istiyor", "Bu ay gece saatlerindeki harcamalar belirginlesmis. Sepeti sabaha birakmak ani kararlari azaltabilir."));

        if (analysis.HighestBudgetUsageCategory.HasValue && analysis.HighestBudgetUsageRate >= 0.80m)
            insights.Add(("Butce sinirina yaklasildi", $"{analysis.HighestBudgetUsageCategory.Value} kategorisi butcesinin {ToPercent(analysis.HighestBudgetUsageRate)} seviyesine gelmis. Ay sonuna kadar daha kontrollu ilerlemek iyi olabilir."));

        if (analysis.MonthlyExpenseIncreaseRate >= 0.30m)
            insights.Add(("Harcama artisi var", $"Toplam harcama gecen aya gore {ToPercent(analysis.MonthlyExpenseIncreaseRate)} artmis. En cok artan kategorileri kontrol etmek hedeflerine alan acabilir."));

        if (analysis.TotalIncome > 0 && analysis.SavingsRate < 0.10m)
            insights.Add(("Birikim payi dusuk", "Bu ay birikim payi dusuk kalmis. Gelir geldigi gun otomatik kucuk bir birikim ayirmak daha surdurulebilir olabilir."));

        if (insights.Count == 0)
            insights.Add(("Denge iyi gidiyor", "Bu ay gelir, harcama ve butce dengesi sakin gorunuyor. Bu ritmi koruyup hedeflerin icin kucuk birikim adimlari ekleyebilirsin."));

        return insights;
    }

    private async Task<string> TryCreateCoachingMessageAsync(string title, string fallbackMessage, BehaviorAnalysisDto analysis)
    {
        var summary =
            "Baslik: " + title + Environment.NewLine +
            "Kural mesaji: " + fallbackMessage + Environment.NewLine +
            "Aylik gelir: " + analysis.TotalIncome.ToString("0.##", CultureInfo.InvariantCulture) + Environment.NewLine +
            "Aylik harcama: " + analysis.TotalExpense.ToString("0.##", CultureInfo.InvariantCulture) + Environment.NewLine +
            "Birikim orani: " + ToPercent(analysis.SavingsRate) + Environment.NewLine +
            "Maas sonrasi ilk 48 saat harcama orani: " + ToPercent(analysis.SalaryFirst48HourSpendingRate) + Environment.NewLine +
            "Gece harcama orani: " + ToPercent(analysis.NightSpendingRate) + Environment.NewLine +
            "Gecen aya gore harcama artisi: " + ToPercent(analysis.MonthlyExpenseIncreaseRate) + Environment.NewLine +
            "Bu mesaji yargilayici olmayan, destekleyici ve en fazla 3 cumle olacak sekilde yeniden yaz.";

        try
        {
            var message = await _openAIService.GenerateCoachingMessageAsync(summary);
            return string.IsNullOrWhiteSpace(message) ? fallbackMessage : message;
        }
        catch
        {
            return fallbackMessage;
        }
    }

    private static decimal Divide(decimal value, decimal divisor)
    {
        return divisor == 0 ? 0 : Math.Round(value / divisor, 4);
    }

    private static string ToPercent(decimal value)
    {
        return (value * 100).ToString("0.#", CultureInfo.InvariantCulture) + "%";
    }

    private static InsightDto ToDto(Domain.Entities.Insight insight)
    {
        return new InsightDto
        {
            Id = insight.Id,
            Title = insight.Title,
            Message = insight.Message,
            IsRead = insight.IsRead,
            CreatedAt = insight.CreatedAt
        };
    }
}
