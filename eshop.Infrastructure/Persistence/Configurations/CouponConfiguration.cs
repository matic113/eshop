using eshop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eshop.Infrastructure.Persistence.Configurations
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasIndex(c => c.CouponCode)
                .IsUnique();

            builder.Property(c => c.CouponCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.DiscountValue)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.MaxDiscount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.CouponType)
                .IsRequired()
                .HasConversion<string>();
        }
    }
}
