using Güvenior.Application.Common.Interfaces;
using Güvenior.Application.DTOs.Auth;
using Güvenior.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Güvenior.Application.Features.Auth;

public class AuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;

    public AuthService(
        UserManager<User> userManager,
        IJwtService jwtService,
        IEmailService emailService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _emailService = emailService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            MonthlyIncome = dto.MonthlyIncome,
            SalaryDay = dto.SalaryDay
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(" | ", result.Errors.Select(x => x.Description));
            throw new Exception(errors);
        }

        var token = _jwtService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            FullName = user.FullName,
            MonthlyIncome = user.MonthlyIncome,
            SalaryDay = user.SalaryDay
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
            throw new Exception("Kullanici yok");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!isPasswordValid)
            throw new Exception("Sifre yanlis");

        var token = _jwtService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            FullName = user.FullName,
            MonthlyIncome = user.MonthlyIncome,
            SalaryDay = user.SalaryDay
        };
    }

    public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null || string.IsNullOrWhiteSpace(user.Email))
            return;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);

        await _emailService.SendPasswordResetEmailAsync(user, encodedToken);
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
            throw new Exception("Gecersiz islem.");

        var decodedToken = Uri.UnescapeDataString(dto.Token);
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(" | ", result.Errors.Select(x => x.Description));
            throw new Exception(errors);
        }
    }

    public async Task<User> UpdateSalaryAsync(string userId, UpdateSalaryDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new Exception("Kullanici bulunamadi");

        user.MonthlyIncome = dto.MonthlyIncome;
        user.SalaryDay = dto.SalaryDay;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new Exception("Maas guncellenemedi");

        return user;
    }

    public async Task<AuthResponseDto> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new Exception("Kullanici bulunamadi");

        return new AuthResponseDto
        {
            Token = string.Empty,
            Email = user.Email!,
            FullName = user.FullName,
            MonthlyIncome = user.MonthlyIncome,
            SalaryDay = user.SalaryDay
        };
    }
}
