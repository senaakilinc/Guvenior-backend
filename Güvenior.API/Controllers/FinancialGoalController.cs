using System.Security.Claims;
using Güvenior.Application.DTOs.FinancialGoal;
using Güvenior.Application.Features.FinancialGoal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Güvenior.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinancialGoalController : ControllerBase
{
    private readonly FinancialGoalService _financialGoalService;

    public FinancialGoalController(FinancialGoalService financialGoalService)
    {
        _financialGoalService = financialGoalService;
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateFinancialGoalDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var goal = await _financialGoalService.AddAsync(userId, dto);
            return Ok(goal);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMyGoals()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var goals = await _financialGoalService.GetByUserIdAsync(userId);
        return Ok(goals);
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFinancialGoalDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var goal = await _financialGoalService.UpdateAsync(id, userId, dto);
            if (goal == null)
                return NotFound("Hedef bulunamadi.");

            return Ok(goal);
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
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var deleted = await _financialGoalService.DeleteAsync(id, userId);
        if (!deleted)
            return NotFound("Hedef bulunamadi.");

        return NoContent();
    }

    [HttpGet("{id:int}/simulation")]
    public async Task<IActionResult> Simulate(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var simulation = await _financialGoalService.SimulateAsync(id, userId);
        if (simulation == null)
            return NotFound("Hedef bulunamadi.");

        return Ok(simulation);
    }

    [HttpGet("simulations")]
    public async Task<IActionResult> SimulateAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var simulations = await _financialGoalService.SimulateAllAsync(userId);
        return Ok(simulations);
    }

    [HttpPost("spending-impact")]
    public async Task<IActionResult> SimulateSpendingImpact([FromBody] SpendingImpactRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var response = await _financialGoalService.SimulateSpendingImpactAsync(userId, dto);
            if (response == null)
                return NotFound("Hedef bulunamadi.");

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
