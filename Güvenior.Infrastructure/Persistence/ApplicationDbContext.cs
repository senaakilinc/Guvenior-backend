using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Güvenior.Domain.Entities;

namespace Güvenior.Infrastructure.Persistence;

// Burası artık IdentityDbContext<User> oldu. 
// Bu sayede "Users" tablosu zaten Identity tarafından yönetilecek.
public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // DİKKAT: DbSet<User> yazmana artık gerek yok, IdentityDbContext bunu senin için hallediyor.
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Identity tablolarının konfigürasyonu için bu satır ÇOK önemlidir, silme:
        base.OnModelCreating(modelBuilder);
        
        // Buraya ileride Transaction tablosu için özel kısıtlamalar ekleyebiliriz.
    }
}