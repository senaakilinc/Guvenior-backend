using Güvenior.Application.Common.Interfaces;
using Güvenior.Application.DTOs.Auth;
using Güvenior.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Güvenior.Application.Features.Auth;

public class AuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;

    public AuthService(
        UserManager<User> userManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
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
            FullName = user.FullName
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
            throw new Exception("Kullanıcı yok");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!isPasswordValid)
            throw new Exception("Şifre yanlış");

        var token = _jwtService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            FullName = user.FullName
        };
    }
}