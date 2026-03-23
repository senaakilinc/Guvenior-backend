using Güvenior.Application.DTOs.Budget;
using Güvenior.Application.Features.Budget;
using Microsoft.AspNetCore.Mvc;

namespace Güvenior.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetController : ControllerBase
{
    private readonly BudgetService _budgetService;

    public BudgetController(BudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    [HttpPost]
    public async Task<IActionResult> AddOrUpdate([FromBody] CreateBudgetDto dto)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (userId == null) return Unauthorized();

        await _budgetService.AddOrUpdateAsync(userId, dto);
        return Ok(new { message = "Bütçe kaydedildi." });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirst("userId")?.Value;
        if (userId == null) return Unauthorized();

        var budgets = await _budgetService.GetByUserIdAsync(userId);
        return Ok(budgets);
    }
}