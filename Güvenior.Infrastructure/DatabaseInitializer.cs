using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Güvenior.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static void MigrateDatabase(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
        var context = services.GetRequiredService<ApplicationDbContext>();

        try
        {
            logger.LogInformation("Veritabanı kontrol ediliyor...");
            
            // Bekleyen migration'ları kontrol et ve uygula
            if (context.Database.GetPendingMigrations().Any())
            {
                logger.LogInformation("Yeni değişiklikler bulundu, veritabanı güncelleniyor...");
                context.Database.Migrate();
                logger.LogInformation("Veritabanı başarıyla güncellendi!");
            }
            else
            {
                logger.LogInformation("Veritabanı zaten güncel.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Veritabanı güncellenirken bir hata oluştu!");
        }
    }
}