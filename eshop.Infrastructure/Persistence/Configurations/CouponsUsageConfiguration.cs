using eshop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eshop.Infrastructure.Persistence.Configurations
{
    public class CouponsUsageConfiguration : IEntityTypeConfiguration<CouponsUsage>
    {
        public void Configure(EntityTypeBuilder<CouponsUsage> builder)
        {
            builder.HasKey(cu => cu.Id);
            
            builder.HasIndex(cu => new { cu.UserId, cu.CouponId })
                .IsUnique();

            // Relationships
            builder.HasOne(cu => cu.Coupon)
                .WithMany(c => c.CouponsUsages)
                .HasForeignKey(cu => cu.CouponId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(cu => cu.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
