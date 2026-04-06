using System.Security.Claims;
using Güvenior.Application.DTOs.Expense;
using Güvenior.Application.Features.Expense;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Güvenior.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RecurringExpenseController : ControllerBase
{
    private readonly RecurringExpenseService _service;

    public RecurringExpenseController(RecurringExpenseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var expenses = await _service.GetAllAsync(userId);
        return Ok(expenses);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRecurringExpenseDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var expense = await _service.CreateAsync(userId, dto);
        return Ok(expense);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        await _service.DeleteAsync(id, userId);
        return NoContent();
    }
}
