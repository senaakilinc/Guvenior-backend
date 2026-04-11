using System.Security.Claims;
using Güvenior.Application.DTOs.Expense;
using Güvenior.Application.Features.Expense;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Güvenior.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpenseController : ControllerBase
{
    private readonly ExpenseService _expenseService;

    public ExpenseController(ExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateExpenseDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        await _expenseService.AddAsync(userId, dto);
        return Ok("Harcama eklendi");
    }

    [HttpGet]
    public async Task<IActionResult> GetMyExpenses()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var expenses = await _expenseService.GetByUserIdAsync(userId);
        return Ok(expenses);
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateExpenseDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var expense = await _expenseService.UpdateAsync(id, userId, dto);
        if (expense == null)
            return NotFound("Harcama bulunamadı.");

        return Ok(expense);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var deleted = await _expenseService.DeleteAsync(id, userId);
        if (!deleted)
            return NotFound("Harcama bulunamadı.");

        return NoContent();
    }
}
