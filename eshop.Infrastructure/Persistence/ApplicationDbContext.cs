using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using eshop.Domain.Entities;
using eshop.Domain.NonKeyed;
using System.Reflection.Emit;

namespace eshop.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply Configurations for models
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        // Identity Tables are added by base class IdentityDbContext
        public DbSet<VerificationToken> VerificationTokens { get; set; } = null!;
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Offer> Offers { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductPicture> ProductPictures { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Seller> Sellers { get; set; } = null!;

        // No Key
        public DbSet<ProductFtsResult> ProductFtsResults { get; set; } = null!;
    }
}
