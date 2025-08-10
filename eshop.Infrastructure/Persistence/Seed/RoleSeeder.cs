using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace eshop.Infrastructure.Persistence.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            var roles = new[] { "Admin", "Seller", "Buyer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }
        }
    }
}
