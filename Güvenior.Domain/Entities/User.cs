using Microsoft.AspNetCore.Identity;

namespace Güvenior.Domain.Entities;

// IdentityUser sayesinde Id, Email, PasswordHash gibi alanlar otomatik gelir
public class User : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public decimal MonthlyIncome { get; set; }
    public int SalaryDay { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Kullanıcının harcamalarıyla ilişkisi
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}