using Güvenior.Application.DTOs.Auth;
using Güvenior.Application.Features.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Güvenior.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        await _authService.ForgotPasswordAsync(dto);

        return Ok(new
        {
            message = "Eger bu e-posta kayitliysa sifre sifirlama baglantisi gonderildi."
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        await _authService.ResetPasswordAsync(dto);
        return Ok(new { message = "Sifre basariyla yenilendi." });
    }

    [Authorize]
    [HttpPut("update-salary")]
    public async Task<IActionResult> UpdateSalary([FromBody] UpdateSalaryDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _authService.UpdateSalaryAsync(userId, dto);
        return Ok(new { message = "Maas guncellendi.", monthlyIncome = user.MonthlyIncome, salaryDay = user.SalaryDay });
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var profile = await _authService.GetProfileAsync(userId);
        return Ok(profile);
    }
}
