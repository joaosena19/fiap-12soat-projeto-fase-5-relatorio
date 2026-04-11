using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace API.Configurations;

public static class MigrationConfiguration
{
    public static void AplicarMigracoes(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}
