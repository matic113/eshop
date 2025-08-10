using eshop.Infrastructure.Persistence;
using eshop.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace eshop.Infrastructure
{
    public static class ConfigureApp
    {
        public static async Task ConfigurePersistence(this WebApplication app)
        {
            await app.EnsureDatabaseCreated();
        }
        private static async Task EnsureDatabaseCreated(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync();

            await RoleSeeder.SeedRolesAsync(scope.ServiceProvider);
        }
    }
}
