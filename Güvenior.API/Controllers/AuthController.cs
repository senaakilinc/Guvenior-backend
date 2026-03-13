using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Güvenior.Domain.Entities;
using Güvenior.API.DTOs;
using System.IdentityModel.Tokens.Jwt; // Bunu ekle
using System.Security.Claims; // Bunu ekle
using Microsoft.IdentityModel.Tokens; // Bunu ekle
using System.Text; // Bunu ekle

namespace Güvenior.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public AuthController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var user = new User
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            FullName = registerDto.FullName,
            MonthlyIncome = registerDto.MonthlyIncome,
            SalaryDay = registerDto.SalaryDay
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (result.Succeeded)
        {
            return Ok("Kullanıcı başarıyla oluşturuldu.");
        }

        return BadRequest(result.Errors);
    }

    // --- BURASI YENİ: LOGIN METODU ---
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        
        // Kullanıcı var mı ve şifre doğru mu kontrol et
        if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            // Token içine gömülecek bilgiler (Claims)
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id), // UserId çok önemli
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Program.cs'deki anahtarın aynısı olmalı!
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("BuCokGizliVeUzunBirAnahtarCumlesi123!"));

            var token = new JwtSecurityToken(
                issuer: "Guvenior",
                audience: "GuveniorUsers",
                expires: DateTime.Now.AddHours(3), // 3 saat geçerli
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
        
        return Unauthorized("E-posta veya şifre hatalı.");
    }
}