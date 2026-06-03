using System.Security.Claims;
using Güvenior.Application.Features.Insight;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Güvenior.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InsightController : ControllerBase
{
    private readonly InsightService _insightService;

    public InsightController(InsightService insightService)
    {
        _insightService = insightService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyInsights()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var insights = await _insightService.GetByUserIdAsync(userId);
        return Ok(insights);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var response = await _insightService.GenerateForCurrentMonthAsync(userId);
        return Ok(response);
    }

    [HttpPatch("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var updated = await _insightService.MarkAsReadAsync(id, userId);
        if (!updated)
            return NotFound("Insight bulunamadi.");

        return NoContent();
    }
}
