using Güvenior.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Güvenior.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CoachingController : ControllerBase
{
    private readonly IOpenAIService _openAIService;

    public CoachingController(IOpenAIService openAIService)
    {
        _openAIService = openAIService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] string behavioralSummary)
    {
        var message = await _openAIService.GenerateCoachingMessageAsync(behavioralSummary);
        return Ok(new { message });
    }
}
