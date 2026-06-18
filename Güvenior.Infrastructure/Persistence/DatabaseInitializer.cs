using Güvenior.Domain.Entities;
using Güvenior.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Güvenior.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task SeedDemoDataAsync(ApplicationDbContext context, UserManager<User> userManager)
    {
        // Check if demo user already exists
        var demoEmail = "demo@guvenior.com";
        var demoUser = await userManager.FindByEmailAsync(demoEmail);

        if (demoUser != null)
        {
            return; // Demo user already exists, skip seeding
        }

        // Create demo user
        demoUser = new User
        {
            UserName = demoEmail,
            Email = demoEmail,
            FullName = "Ahmet YeniMezun",
            MonthlyIncome = 45000,
            SalaryDay = 15,
            EmailConfirmed = true,
            CreatedDate = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var result = await userManager.CreateAsync(demoUser, "Demo123!");
        if (!result.Succeeded)
        {
            throw new Exception($"Demo user could not be created: {string.Join(", ", result.Errors.Select(x => x.Description))}");
        }

        var userId = demoUser.Id;

        // 1. Seed Budgets for April, May, June 2026
        var budgets = new List<Budget>();
        int[] months = { 4, 5, 6 };
        foreach (var month in months)
        {
            budgets.Add(new Budget { UserId = userId, Category = ExpenseCategory.Food, LimitAmount = 12000, Month = month, Year = 2026 });
            budgets.Add(new Budget { UserId = userId, Category = ExpenseCategory.Shopping, LimitAmount = 8000, Month = month, Year = 2026 });
            budgets.Add(new Budget { UserId = userId, Category = ExpenseCategory.Entertainment, LimitAmount = 6000, Month = month, Year = 2026 });
            budgets.Add(new Budget { UserId = userId, Category = ExpenseCategory.Transport, LimitAmount = 4000, Month = month, Year = 2026 });
            budgets.Add(new Budget { UserId = userId, Category = ExpenseCategory.Bills, LimitAmount = 5000, Month = month, Year = 2026 });
        }
        await context.Budgets.AddRangeAsync(budgets);

        // 2. Seed Incomes
        var incomes = new List<Income>
        {
            new Income { UserId = userId, Title = "Nisan Maaşı", Amount = 45000, ReceivedDate = new DateTime(2026, 4, 15, 9, 0, 0, DateTimeKind.Utc), Type = IncomeType.Salary },
            new Income { UserId = userId, Title = "Mayıs Maaşı", Amount = 45000, ReceivedDate = new DateTime(2026, 5, 15, 9, 0, 0, DateTimeKind.Utc), Type = IncomeType.Salary },
            new Income { UserId = userId, Title = "Haziran Maaşı", Amount = 45000, ReceivedDate = new DateTime(2026, 6, 15, 9, 0, 0, DateTimeKind.Utc), Type = IncomeType.Salary },
            new Income { UserId = userId, Title = "Freelance UI Tasarımı", Amount = 8000, ReceivedDate = new DateTime(2026, 5, 28, 14, 30, 0, DateTimeKind.Utc), Type = IncomeType.Freelance }
        };
        await context.Incomes.AddRangeAsync(incomes);

        // 3. Seed Recurring Expenses
        var recurringExpenses = new List<RecurringExpense>
        {
            new RecurringExpense { UserId = userId, Title = "Ev Kirası", Amount = 18000, Category = ExpenseCategory.Rent, DayOfMonth = 16, IsActive = true, LastGeneratedYear = 2026, LastGeneratedMonth = 6 },
            new RecurringExpense { UserId = userId, Title = "Turknet İnternet", Amount = 450, Category = ExpenseCategory.Bills, DayOfMonth = 18, IsActive = true, LastGeneratedYear = 2026, LastGeneratedMonth = 6 },
            new RecurringExpense { UserId = userId, Title = "MacFit Spor Üyeliği", Amount = 1200, Category = ExpenseCategory.Entertainment, DayOfMonth = 5, IsActive = true, LastGeneratedYear = 2026, LastGeneratedMonth = 6 }
        };
        await context.RecurringExpenses.AddRangeAsync(recurringExpenses);

        // 4. Seed Expenses
        var expenses = new List<Expense>();

        // Rent (April, May, June)
        expenses.Add(new Expense { UserId = userId, Title = "Kira Ödemesi", Amount = 18000, Category = ExpenseCategory.Rent, SpentAt = new DateTime(2026, 4, 16, 10, 0, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Kira Ödemesi", Amount = 18000, Category = ExpenseCategory.Rent, SpentAt = new DateTime(2026, 5, 16, 10, 0, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Kira Ödemesi", Amount = 18000, Category = ExpenseCategory.Rent, SpentAt = new DateTime(2026, 6, 16, 10, 0, 0, DateTimeKind.Utc) });

        // Bills (April, May, June)
        expenses.Add(new Expense { UserId = userId, Title = "Turknet İnternet", Amount = 450, Category = ExpenseCategory.Bills, SpentAt = new DateTime(2026, 4, 18, 11, 0, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Turknet İnternet", Amount = 450, Category = ExpenseCategory.Bills, SpentAt = new DateTime(2026, 5, 18, 11, 0, 0, DateTimeKind.Utc) });

        expenses.Add(new Expense { UserId = userId, Title = "Elektrik Faturası", Amount = 1200, Category = ExpenseCategory.Bills, SpentAt = new DateTime(2026, 4, 20, 15, 0, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Elektrik Faturası", Amount = 1350, Category = ExpenseCategory.Bills, SpentAt = new DateTime(2026, 5, 20, 15, 0, 0, DateTimeKind.Utc) });

        expenses.Add(new Expense { UserId = userId, Title = "Su Faturası", Amount = 350, Category = ExpenseCategory.Bills, SpentAt = new DateTime(2026, 4, 22, 14, 0, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Su Faturası", Amount = 400, Category = ExpenseCategory.Bills, SpentAt = new DateTime(2026, 5, 22, 14, 0, 0, DateTimeKind.Utc) });

        // Gym (May, June)
        expenses.Add(new Expense { UserId = userId, Title = "MacFit Üyelik", Amount = 1200, Category = ExpenseCategory.Entertainment, SpentAt = new DateTime(2026, 5, 5, 9, 30, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "MacFit Üyelik", Amount = 1200, Category = ExpenseCategory.Entertainment, SpentAt = new DateTime(2026, 6, 5, 9, 30, 0, DateTimeKind.Utc) });

        // April Daily Spending (Balanced/Learning)
        expenses.Add(new Expense { UserId = userId, Title = "CarrefourSA Market", Amount = 1850, Category = ExpenseCategory.Food, SpentAt = new DateTime(2026, 4, 3, 18, 0, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Starbucks Kahve", Amount = 180, Category = ExpenseCategory.Food, SpentAt = new DateTime(2026, 4, 5, 14, 20, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "BiTaksi Ulaşım", Amount = 350, Category = ExpenseCategory.Transport, SpentAt = new DateTime(2026, 4, 8, 8, 30, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "D&R Kitap Alışverişi", Amount = 750, Category = ExpenseCategory.Shopping, SpentAt = new DateTime(2026, 4, 12, 16, 45, 0, DateTimeKind.Utc) });
        // After Salary - Balanced
        expenses.Add(new Expense { UserId = userId, Title = "Yemeksepeti Akşam Yemeği", Amount = 480, Category = ExpenseCategory.Food, SpentAt = new DateTime(2026, 4, 15, 20, 0, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Trendyol Kıyafet", Amount = 2400, Category = ExpenseCategory.Shopping, SpentAt = new DateTime(2026, 4, 17, 19, 30, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Sinema Bileti & Mısır", Amount = 380, Category = ExpenseCategory.Entertainment, SpentAt = new DateTime(2026, 4, 19, 21, 0, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Migros Haftalık Market", Amount = 2200, Category = ExpenseCategory.Food, SpentAt = new DateTime(2026, 4, 24, 17, 0, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Kadıköy Cafe Buluşması", Amount = 650, Category = ExpenseCategory.Food, SpentAt = new DateTime(2026, 4, 26, 15, 0, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Metro Bileti", Amount = 150, Category = ExpenseCategory.Transport, SpentAt = new DateTime(2026, 4, 29, 9, 0, 0, DateTimeKind.Utc) });

        // May Daily Spending (Emotional Spending Spree right after salary!)
        expenses.Add(new Expense { UserId = userId, Title = "Migros Market", Amount = 1950, Category = ExpenseCategory.Food, SpentAt = new DateTime(2026, 5, 2, 18, 0, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Uber Ulaşım", Amount = 450, Category = ExpenseCategory.Transport, SpentAt = new DateTime(2026, 5, 6, 22, 10, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Netflix Abonelik", Amount = 230, Category = ExpenseCategory.Entertainment, SpentAt = new DateTime(2026, 5, 10, 8, 0, 0, DateTimeKind.Utc) });
        
        // Right after payday (May 15): Spend Spree (Showcases: SalaryFirst48HourSpendingRate >= 35%)
        expenses.Add(new Expense { UserId = userId, Title = "Nike Store - Air Max", Amount = 5200, Category = ExpenseCategory.Shopping, SpentAt = new DateTime(2026, 5, 15, 14, 0, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Zara Mağazası Alışveriş", Amount = 6400, Category = ExpenseCategory.Shopping, SpentAt = new DateTime(2026, 5, 16, 11, 30, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Nusr-Et Akşam Yemeği", Amount = 4800, Category = ExpenseCategory.Food, SpentAt = new DateTime(2026, 5, 16, 20, 0, 0, DateTimeKind.Utc) });

        // Late Night spending in May (Showcases: NightSpendingRate >= 15%)
        expenses.Add(new Expense { UserId = userId, Title = "Steam Oyun Alımı", Amount = 2400, Category = ExpenseCategory.Entertainment, SpentAt = new DateTime(2026, 5, 18, 1, 30, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Kadıköy Bar/Bira", Amount = 3800, Category = ExpenseCategory.Entertainment, SpentAt = new DateTime(2026, 5, 22, 23, 50, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Amazon Gece Siparişi", Amount = 2500, Category = ExpenseCategory.Shopping, SpentAt = new DateTime(2026, 5, 25, 2, 10, 0, DateTimeKind.Utc) });

        // Other May Expenses
        expenses.Add(new Expense { UserId = userId, Title = "Getir Yemek", Amount = 550, Category = ExpenseCategory.Food, SpentAt = new DateTime(2026, 5, 20, 19, 0, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Taksim Kahve", Amount = 250, Category = ExpenseCategory.Food, SpentAt = new DateTime(2026, 5, 24, 14, 0, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "BiTaksi", Amount = 400, Category = ExpenseCategory.Transport, SpentAt = new DateTime(2026, 5, 27, 18, 0, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Macrocenter Market", Amount = 3100, Category = ExpenseCategory.Food, SpentAt = new DateTime(2026, 5, 30, 16, 0, 0, DateTimeKind.Utc) });

        // June Daily Spending (Current Month, up to June 16)
        expenses.Add(new Expense { UserId = userId, Title = "Migros Sanal Market", Amount = 2500, Category = ExpenseCategory.Food, SpentAt = new DateTime(2026, 6, 2, 15, 0, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Marmaray Ulaşım", Amount = 180, Category = ExpenseCategory.Transport, SpentAt = new DateTime(2026, 6, 4, 8, 15, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Starbucks Kahve", Amount = 210, Category = ExpenseCategory.Food, SpentAt = new DateTime(2026, 6, 6, 11, 0, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Trendyol Kitaplık", Amount = 1850, Category = ExpenseCategory.Shopping, SpentAt = new DateTime(2026, 6, 9, 20, 30, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Eczane Alışverişi", Amount = 650, Category = ExpenseCategory.Other, SpentAt = new DateTime(2026, 6, 12, 13, 0, 0, DateTimeKind.Utc) });
        expenses.Add(new Expense { UserId = userId, Title = "Yemeksepeti Pizza", Amount = 650, Category = ExpenseCategory.Food, SpentAt = new DateTime(2026, 6, 15, 19, 45, 0, DateTimeKind.Utc) });

        await context.Expenses.AddRangeAsync(expenses);

        // 5. Seed Financial Goals
        var goals = new List<FinancialGoal>
        {
            new FinancialGoal
            {
                UserId = userId,
                Title = "Yeni MacBook Pro",
                Type = FinancialGoalType.Other,
                CurrentPrice = 90000,
                CurrentSavings = 25000,
                MonthlyContribution = 6000,
                AnnualInflationRate = 15,
                TargetDate = DateTime.UtcNow.AddMonths(10),
                IsCompleted = false
            },
            new FinancialGoal
            {
                UserId = userId,
                Title = "Avrupa Tatili",
                Type = FinancialGoalType.Other,
                CurrentPrice = 60000,
                CurrentSavings = 10000,
                MonthlyContribution = 4000,
                AnnualInflationRate = 10,
                TargetDate = DateTime.UtcNow.AddMonths(6),
                IsCompleted = false
            }
        };
        await context.FinancialGoals.AddRangeAsync(goals);

        // 6. Seed Pre-calculated Insights
        var insights = new List<Insight>
        {
            new Insight
            {
                UserId = userId,
                Title = "Maaş Sonrası Tempo Yüksek",
                Message = "Maaşın yattığı ilk 48 saatte toplam gelirinin %36.4'ünü harcadın! Gelecek ay harcamalarını yapmadan önce 24 saat kuralını uygulamak bütçeni korumana yardımcı olabilir.",
                IsRead = false
            },
            new Insight
            {
                UserId = userId,
                Title = "Gece Harcamaları Dikkat İstiyor",
                Message = "Geçtiğimiz ay harcamalarının %17.4'ünü gece 23:00 - 05:00 saatleri arasında (özellikle Steam ve online siparişlerde) yaptın. Kararlarını sabaha ertelemek dürtüsel harcamalarını azaltabilir.",
                IsRead = false
            },
            new Insight
            {
                UserId = userId,
                Title = "Alışveriş Bütçesi Sınırda",
                Message = "Alışveriş kategorisi bütçenin %111'ine ulaşarak limiti aştı. Ayın geri kalanında bu kategorideki harcamalarını erteleyip hedeflerine odaklanabilirsin.",
                IsRead = false
            }
        };
        await context.Insights.AddRangeAsync(insights);

        // Save everything
        await context.SaveChangesAsync();
    }
}