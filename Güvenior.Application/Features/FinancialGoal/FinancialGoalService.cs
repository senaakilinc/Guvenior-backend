using Güvenior.Application.Common.Interfaces;
using Güvenior.Application.DTOs.FinancialGoal;
using Güvenior.Domain.Enums;
using FinancialGoalEntity = Güvenior.Domain.Entities.FinancialGoal;
using ExpenseEntity = Güvenior.Domain.Entities.Expense;

namespace Güvenior.Application.Features.FinancialGoal;

public class FinancialGoalService
{
    private readonly IFinancialGoalRepository _financialGoalRepository;
    private readonly IMarketAssumptionService _marketAssumptionService;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IIncomeRepository _incomeRepository;
    private readonly IOpenAIService _openAIService;

    public FinancialGoalService(
        IFinancialGoalRepository financialGoalRepository,
        IMarketAssumptionService marketAssumptionService,
        IExpenseRepository expenseRepository,
        IIncomeRepository incomeRepository,
        IOpenAIService openAIService)
    {
        _financialGoalRepository = financialGoalRepository;
        _marketAssumptionService = marketAssumptionService;
        _expenseRepository = expenseRepository;
        _incomeRepository = incomeRepository;
        _openAIService = openAIService;
    }

    public async Task<FinancialGoalDto> AddAsync(string userId, CreateFinancialGoalDto dto)
    {
        var annualInflationRate = dto.AnnualInflationRate
            ?? _marketAssumptionService.GetDefaultAnnualInflationRate(dto.Type);

        Validate(dto.CurrentPrice, dto.CurrentSavings, dto.MonthlyContribution, annualInflationRate, dto.TargetDate);

        var goal = new FinancialGoalEntity
        {
            UserId = userId,
            Title = dto.Title,
            Type = dto.Type,
            CurrentPrice = dto.CurrentPrice,
            CurrentSavings = dto.CurrentSavings,
            MonthlyContribution = dto.MonthlyContribution,
            AnnualInflationRate = annualInflationRate,
            TargetDate = DateTime.SpecifyKind(dto.TargetDate, DateTimeKind.Utc)
        };

        await _financialGoalRepository.AddAsync(goal);
        return ToDto(goal);
    }

    public async Task<List<FinancialGoalDto>> GetByUserIdAsync(string userId)
    {
        var goals = await _financialGoalRepository.GetByUserIdAsync(userId);
        return goals.Select(ToDto).ToList();
    }

    public async Task<FinancialGoalDto?> UpdateAsync(int id, string userId, UpdateFinancialGoalDto dto)
    {
        var goal = await _financialGoalRepository.GetByIdAsync(id);
        if (goal == null || goal.UserId != userId)
            return null;

        if (!string.IsNullOrWhiteSpace(dto.Title))
            goal.Title = dto.Title;

        if (dto.Type.HasValue)
            goal.Type = dto.Type.Value;

        if (dto.CurrentPrice.HasValue)
            goal.CurrentPrice = dto.CurrentPrice.Value;

        if (dto.CurrentSavings.HasValue)
            goal.CurrentSavings = dto.CurrentSavings.Value;

        if (dto.MonthlyContribution.HasValue)
            goal.MonthlyContribution = dto.MonthlyContribution.Value;

        if (dto.AnnualInflationRate.HasValue)
            goal.AnnualInflationRate = dto.AnnualInflationRate.Value;

        if (dto.TargetDate.HasValue)
            goal.TargetDate = DateTime.SpecifyKind(dto.TargetDate.Value, DateTimeKind.Utc);

        if (dto.IsCompleted.HasValue)
            goal.IsCompleted = dto.IsCompleted.Value;

        Validate(goal.CurrentPrice, goal.CurrentSavings, goal.MonthlyContribution, goal.AnnualInflationRate, goal.TargetDate);

        await _financialGoalRepository.UpdateAsync(goal);
        return ToDto(goal);
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        var goal = await _financialGoalRepository.GetByIdAsync(id);
        if (goal == null || goal.UserId != userId)
            return false;

        await _financialGoalRepository.DeleteAsync(goal);
        return true;
    }

    public async Task<FinancialGoalSimulationDto?> SimulateAsync(int id, string userId)
    {
        var goal = await _financialGoalRepository.GetByIdAsync(id);
        if (goal == null || goal.UserId != userId)
            return null;

        return await SimulateWithBehaviorAsync(goal, userId);
    }

    public async Task<List<FinancialGoalSimulationDto>> SimulateAllAsync(string userId)
    {
        var goals = await _financialGoalRepository.GetByUserIdAsync(userId);
        var simulations = new List<FinancialGoalSimulationDto>();

        foreach (var goal in goals.Where(x => !x.IsCompleted))
        {
            simulations.Add(await SimulateWithBehaviorAsync(goal, userId));
        }

        return simulations
            .OrderByDescending(x => x.Analysis.RiskLevel == "Yuksek")
            .ThenByDescending(x => x.Analysis.RiskLevel == "Orta")
            .ToList();
    }

    public async Task<SpendingImpactResponseDto?> SimulateSpendingImpactAsync(string userId, SpendingImpactRequestDto dto)
    {
        if (dto.Amount <= 0)
            throw new InvalidOperationException("Harcama tutari sifirdan buyuk olmalidir.");

        var goal = await _financialGoalRepository.GetByIdAsync(dto.GoalId);
        if (goal == null || goal.UserId != userId)
            return null;

        var baseAnalysis = BuildBaseAnalysis(goal);
        var newFundingGap = Math.Round(baseAnalysis.FundingGap + dto.Amount, 2);
        var estimatedDelayMonths = goal.MonthlyContribution <= 0
            ? 0
            : (int)Math.Ceiling(dto.Amount / goal.MonthlyContribution);
        var suggestedMonthlyOffset = Math.Round(dto.Amount / Math.Max(1, baseAnalysis.MonthsRemaining), 2);

        var analysis = new SpendingImpactAnalysisDto
        {
            GoalId = goal.Id,
            GoalTitle = goal.Title,
            ExpenseTitle = dto.Title,
            ExpenseCategory = dto.Category,
            ExpenseCategoryName = Güvenior.Application.Common.Helpers.DisplayNameHelper.ToDisplayName(dto.Category),
            ExpenseAmount = dto.Amount,
            OriginalFundingGap = baseAnalysis.FundingGap,
            NewFundingGap = newFundingGap,
            ImpactAmount = dto.Amount,
            EstimatedDelayMonths = estimatedDelayMonths,
            SuggestedMonthlyOffset = suggestedMonthlyOffset,
            ImpactLevel = GetSpendingImpactLevel(dto.Amount, goal.MonthlyContribution)
        };

        var ruleBasedRecommendation = BuildSpendingImpactRecommendation(analysis);
        var aiMessage = await BuildSpendingImpactAiMessageAsync(analysis, ruleBasedRecommendation);

        return new SpendingImpactResponseDto
        {
            Analysis = analysis,
            RuleBasedRecommendation = ruleBasedRecommendation,
            AiMessage = aiMessage
        };
    }

    private async Task<FinancialGoalSimulationDto> SimulateWithBehaviorAsync(FinancialGoalEntity goal, string userId)
    {
        var analysis = BuildBaseAnalysis(goal);
        var now = DateTime.UtcNow;
        var expenses = await _expenseRepository.GetByUserIdAsync(userId);
        var incomes = await _incomeRepository.GetByUserIdAsync(userId);

        var currentMonthExpenses = expenses
            .Where(x => x.SpentAt.Month == now.Month && x.SpentAt.Year == now.Year)
            .ToList();
        var currentMonthIncome = incomes
            .Where(x => x.ReceivedDate.Month == now.Month && x.ReceivedDate.Year == now.Year)
            .Sum(x => x.Amount);
        var currentMonthExpense = currentMonthExpenses.Sum(x => x.Amount);
        var cutSuggestion = FindCutSuggestion(currentMonthExpenses, analysis.MonthlyContributionDifference);

        analysis.CurrentMonthIncome = currentMonthIncome;
        analysis.CurrentMonthExpense = currentMonthExpense;
        analysis.CurrentMonthSavingsCapacity = currentMonthIncome - currentMonthExpense;
        analysis.SuggestedCutCategory = cutSuggestion.Category;
        analysis.SuggestedMonthlyExpenseReduction = cutSuggestion.Amount;
        analysis.AdjustedFundingGap = Math.Max(0, analysis.FundingGap - (cutSuggestion.Amount * analysis.MonthsRemaining));

        var ruleBasedRecommendation = BuildRuleBasedRecommendation(analysis);
        var aiMessage = await BuildAiMessageAsync(goal, analysis, ruleBasedRecommendation);

        return new FinancialGoalSimulationDto
        {
            GoalId = goal.Id,
            Title = goal.Title,
            Analysis = analysis,
            RuleBasedRecommendation = ruleBasedRecommendation,
            AiMessage = aiMessage
        };
    }

    private static (string? Category, decimal Amount) FindCutSuggestion(List<ExpenseEntity> currentMonthExpenses, decimal monthlyContributionDifference)
    {
        if (monthlyContributionDifference <= 0 || currentMonthExpenses.Count == 0)
            return (null, 0);

        var flexibleCategories = new[]
        {
            ExpenseCategory.Shopping,
            ExpenseCategory.Entertainment,
            ExpenseCategory.Food,
            ExpenseCategory.Transport,
            ExpenseCategory.Other
        };

        var highestFlexibleCategory = currentMonthExpenses
            .Where(x => flexibleCategories.Contains(x.Category))
            .GroupBy(x => x.Category)
            .Select(x => new
            {
                Category = x.Key,
                Total = x.Sum(expense => expense.Amount)
            })
            .OrderByDescending(x => x.Total)
            .FirstOrDefault();

        if (highestFlexibleCategory == null || highestFlexibleCategory.Total <= 0)
            return (null, 0);

        var suggestedReduction = Math.Min(monthlyContributionDifference, highestFlexibleCategory.Total * 0.15m);
        return (Güvenior.Application.Common.Helpers.DisplayNameHelper.ToDisplayName(highestFlexibleCategory.Category), Math.Round(suggestedReduction, 2));
    }

    private async Task<string> BuildAiMessageAsync(
        FinancialGoalEntity goal,
        FinancialGoalSimulationAnalysisDto analysis,
        string ruleBasedRecommendation)
    {
        var summary =
            $"Hedef: {goal.Title}\n" +
            $"Hedef tipi: {Güvenior.Application.Common.Helpers.DisplayNameHelper.ToDisplayName(goal.Type)}\n" +
            $"Risk seviyesi: {analysis.RiskLevel}\n" +
            $"Hedefe kalan ay: {analysis.MonthsRemaining}\n" +
            $"Enflasyonlu hedef tutar: {analysis.ProjectedTargetPrice:0.##} TL\n" +
            $"Mevcut planla acik: {analysis.FundingGap:0.##} TL\n" +
            $"Gereken aylik birikim: {analysis.RequiredMonthlyContribution:0.##} TL\n" +
            $"Mevcut aylik birikimden fark: {analysis.MonthlyContributionDifference:0.##} TL\n" +
            $"Bu ay gelir: {analysis.CurrentMonthIncome:0.##} TL\n" +
            $"Bu ay harcama: {analysis.CurrentMonthExpense:0.##} TL\n" +
            $"Bu ay kalan kapasite: {analysis.CurrentMonthSavingsCapacity:0.##} TL\n" +
            $"Esneme onerilen kategori: {analysis.SuggestedCutCategory ?? "Yok"}\n" +
            $"Onerilen aylik azaltim: {analysis.SuggestedMonthlyExpenseReduction:0.##} TL\n" +
            $"Kural onerisi: {ruleBasedRecommendation}\n" +
            "Bu verileri kullanarak yargilamayan, destekleyici, finansal danismanlik iddiasi olmayan, en fazla 3 cumlelik hedef odakli mesaj yaz. Sayilari degistirme veya yeni sayi uydurma.";

        try
        {
            var message = await _openAIService.GenerateCoachingMessageAsync(summary);
            return string.IsNullOrWhiteSpace(message) ? string.Empty : message;
        }
        catch
        {
            return string.Empty;
        }
    }

    private async Task<string> BuildSpendingImpactAiMessageAsync(
        SpendingImpactAnalysisDto analysis,
        string ruleBasedRecommendation)
    {
        var summary =
            $"Hedef: {analysis.GoalTitle}\n" +
            $"Planlanan harcama: {analysis.ExpenseTitle}\n" +
            $"Harcama kategorisi: {Güvenior.Application.Common.Helpers.DisplayNameHelper.ToDisplayName(analysis.ExpenseCategory)}\n" +
            $"Harcama tutari: {analysis.ExpenseAmount:0.##} TL\n" +
            $"Mevcut hedef acigi: {analysis.OriginalFundingGap:0.##} TL\n" +
            $"Harcama sonrasi hedef acigi: {analysis.NewFundingGap:0.##} TL\n" +
            $"Tahmini gecikme: {analysis.EstimatedDelayMonths} ay\n" +
            $"Aylik telafi onerisi: {analysis.SuggestedMonthlyOffset:0.##} TL\n" +
            $"Etki seviyesi: {analysis.ImpactLevel}\n" +
            $"Kural onerisi: {ruleBasedRecommendation}\n" +
            "Bu verilerle kullaniciya yargilamayan, destekleyici, karar oncesi dusundurucu, en fazla 3 cumlelik mesaj yaz. Sayilari degistirme veya yeni sayi uydurma.";

        var message = await _openAIService.GenerateCoachingMessageAsync(summary);
        return string.IsNullOrWhiteSpace(message) ? string.Empty : message;
    }

    private static FinancialGoalSimulationAnalysisDto BuildBaseAnalysis(FinancialGoalEntity goal)
    {
        var today = DateTime.UtcNow.Date;
        var targetDate = goal.TargetDate.Date;
        var monthsRemaining = Math.Max(1, ((targetDate.Year - today.Year) * 12) + targetDate.Month - today.Month);
        var yearsRemaining = monthsRemaining / 12m;
        var inflationMultiplier = (decimal)Math.Pow((double)(1 + (goal.AnnualInflationRate / 100m)), (double)yearsRemaining);
        var projectedTargetPrice = Math.Round(goal.CurrentPrice * inflationMultiplier, 2);
        var plannedTotalSavings = Math.Round(goal.CurrentSavings + (goal.MonthlyContribution * monthsRemaining), 2);
        var fundingGap = Math.Max(0, Math.Round(projectedTargetPrice - plannedTotalSavings, 2));
        var requiredMonthlyContribution = Math.Max(0, Math.Round((projectedTargetPrice - goal.CurrentSavings) / monthsRemaining, 2));
        var monthlyContributionDifference = Math.Round(requiredMonthlyContribution - goal.MonthlyContribution, 2);
        var isReachable = plannedTotalSavings >= projectedTargetPrice;
        var coverageRate = projectedTargetPrice <= 0 ? 1 : plannedTotalSavings / projectedTargetPrice;

        return new FinancialGoalSimulationAnalysisDto
        {
            MonthsRemaining = monthsRemaining,
            CurrentPrice = goal.CurrentPrice,
            ProjectedTargetPrice = projectedTargetPrice,
            CurrentSavings = goal.CurrentSavings,
            PlannedTotalSavings = plannedTotalSavings,
            FundingGap = fundingGap,
            RequiredMonthlyContribution = requiredMonthlyContribution,
            MonthlyContributionDifference = monthlyContributionDifference,
            IsReachableWithCurrentPlan = isReachable,
            RiskLevel = GetRiskLevel(coverageRate),
            AdjustedFundingGap = fundingGap
        };
    }

    private static string GetRiskLevel(decimal coverageRate)
    {
        if (coverageRate >= 1)
            return "Dusuk";

        if (coverageRate >= 0.75m)
            return "Orta";

        return "Yuksek";
    }

    private static string BuildRuleBasedRecommendation(FinancialGoalSimulationAnalysisDto analysis)
    {
        if (analysis.IsReachableWithCurrentPlan)
            return "Mevcut plan hedefe ulasmak icin yeterli gorunuyor. Bu tempoyu korumak onemli.";

        if (!string.IsNullOrWhiteSpace(analysis.SuggestedCutCategory) && analysis.SuggestedMonthlyExpenseReduction > 0)
        {
            return $"{analysis.SuggestedCutCategory} kategorisinden aylik yaklasik {analysis.SuggestedMonthlyExpenseReduction:0.##} TL azaltim, hedef acigini {analysis.AdjustedFundingGap:0.##} TL seviyesine indirebilir. Aylik birikimi toplamda {Math.Max(0, analysis.MonthlyContributionDifference):0.##} TL artirmak hedefi daha gercekci hale getirir.";
        }

        return $"Bu hedef icin mevcut plana gore {analysis.FundingGap:0.##} TL acik var. Aylik birikimi yaklasik {Math.Max(0, analysis.MonthlyContributionDifference):0.##} TL artirmak hedefi daha gercekci hale getirir.";
    }

    private static string BuildSpendingImpactRecommendation(SpendingImpactAnalysisDto analysis)
    {
        if (analysis.EstimatedDelayMonths <= 0)
        {
            return $"{analysis.ExpenseTitle} harcamasi hedef acigini {analysis.NewFundingGap:0.##} TL seviyesine cikarir. Bu etkiyi azaltmak icin hedef tarihine kadar aylik yaklasik {analysis.SuggestedMonthlyOffset:0.##} TL ek birikim ayirmak yeterli olabilir.";
        }

        return $"{analysis.ExpenseTitle} harcamasi {analysis.GoalTitle} hedefini yaklasik {analysis.EstimatedDelayMonths} ay geciktirebilir. Etkiyi dengelemek icin hedef tarihine kadar aylik yaklasik {analysis.SuggestedMonthlyOffset:0.##} TL ek birikim ayirmak iyi olabilir.";
    }

    private static string GetSpendingImpactLevel(decimal expenseAmount, decimal monthlyContribution)
    {
        if (monthlyContribution <= 0)
            return "Bilinmiyor";

        var ratio = expenseAmount / monthlyContribution;
        if (ratio < 0.5m)
            return "Dusuk";

        if (ratio <= 1.5m)
            return "Orta";

        return "Yuksek";
    }

    private static void Validate(decimal currentPrice, decimal currentSavings, decimal monthlyContribution, decimal annualInflationRate, DateTime targetDate)
    {
        if (currentPrice <= 0)
            throw new InvalidOperationException("Hedef tutari sifirdan buyuk olmalidir.");

        if (currentSavings < 0 || monthlyContribution < 0)
            throw new InvalidOperationException("Birikim ve aylik katki negatif olamaz.");

        if (annualInflationRate < 0)
            throw new InvalidOperationException("Enflasyon orani negatif olamaz.");

        if (targetDate.Date <= DateTime.UtcNow.Date)
            throw new InvalidOperationException("Hedef tarihi bugunden ileri bir tarih olmalidir.");
    }

    private static FinancialGoalDto ToDto(FinancialGoalEntity goal)
    {
        return new FinancialGoalDto
        {
            Id = goal.Id,
            Title = goal.Title,
            Type = goal.Type,
            CurrentPrice = goal.CurrentPrice,
            CurrentSavings = goal.CurrentSavings,
            MonthlyContribution = goal.MonthlyContribution,
            AnnualInflationRate = goal.AnnualInflationRate,
            TargetDate = goal.TargetDate,
            IsCompleted = goal.IsCompleted,
            CreatedAt = goal.CreatedAt
        };
    }
}
