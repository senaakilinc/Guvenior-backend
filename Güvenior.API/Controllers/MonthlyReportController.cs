using System.Security.Claims;
using Güvenior.Application.Features.MonthlyReport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Güvenior.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MonthlyReportController : ControllerBase
{
    private readonly MonthlyReportService _monthlyReportService;

    public MonthlyReportController(MonthlyReportService monthlyReportService)
    {
        _monthlyReportService = monthlyReportService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? month, [FromQuery] int? year)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (month.HasValue && (month.Value < 1 || month.Value > 12))
            return BadRequest(new { message = "Ay 1 ile 12 arasinda olmalidir." });

        var report = await _monthlyReportService.GenerateAsync(userId, month, year);
        return Ok(report);
    }
}
