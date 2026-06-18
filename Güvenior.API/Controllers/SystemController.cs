using Güvenior.Domain.Entities;
using Güvenior.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Güvenior.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public SystemController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost("reset-demo")]
    public async Task<IActionResult> ResetDemo()
    {
        var demoEmail = "demo@guvenior.com";
        var demoUser = await _userManager.FindByEmailAsync(demoEmail);

        if (demoUser != null)
        {
            // Delete the demo user. This will cascade delete incomes, expenses, budgets, recurring expenses, insights, and goals.
            var deleteResult = await _userManager.DeleteAsync(demoUser);
            if (!deleteResult.Succeeded)
            {
                return BadRequest(new { message = "Demo kullanıcısı sıfırlanırken silinemedi." });
            }
        }

        // Run seed method to recreate user and data
        await DatabaseInitializer.SeedDemoDataAsync(_context, _userManager);

        return Ok(new { message = "Demo verileri başarıyla sıfırlandı.", email = demoEmail, password = "Demo123!" });
    }
}
