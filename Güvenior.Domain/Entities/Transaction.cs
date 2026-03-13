namespace Güvenior.Domain.Entities;

public class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; } = "Genel";
    public string Description { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    
    // IdentityUser Id'leri string (GUID) olduğu için burayı string yapmalısın
    public string UserId { get; set; } = string.Empty; 
    public User User { get; set; } = null!;
}