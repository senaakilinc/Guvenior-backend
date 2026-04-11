using System.Security.Claims;
using Güvenior.Application.DTOs.Budget;
using Güvenior.Application.Features.Budget;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Güvenior.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await _budgetService.AddOrUpdateAsync(userId, dto);
        return Ok(new { message = "Bütçe kaydedildi." });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var budgets = await _budgetService.GetByUserIdAsync(userId);
        return Ok(budgets);
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBudgetDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var budget = await _budgetService.UpdateAsync(id, userId, dto);
            if (budget == null) return NotFound("Bütçe bulunamadı.");

            return Ok(budget);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var deleted = await _budgetService.DeleteAsync(id, userId);
        if (!deleted) return NotFound("Bütçe bulunamadı.");

        return NoContent();
    }
}
