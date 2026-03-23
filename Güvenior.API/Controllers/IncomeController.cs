using System.Security.Claims;
using Güvenior.Application.DTOs.Income;
using Güvenior.Application.Features.Income;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Güvenior.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IncomeController : ControllerBase
{
    private readonly IncomeService _incomeService;

    public IncomeController(IncomeService incomeService)
    {
        _incomeService = incomeService;
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateIncomeDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        await _incomeService.AddAsync(userId, dto);
        return Ok("Gelir eklendi");
    }

    [HttpGet]
    public async Task<IActionResult> GetMyIncomes()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var incomes = await _incomeService.GetByUserIdAsync(userId);
        return Ok(incomes);
    }
}