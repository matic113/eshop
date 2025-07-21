using eshop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eshop.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.HasIndex(o => o.OrderNumber);

            builder.HasIndex(o => o.UserId);

            builder.Property(o => o.Status)
                .HasConversion<string>();

            builder.Property(o => o.PaymentMethod)
                .HasConversion<string>();

            builder.Property(o => o.TotalPrice)
                .HasColumnType("decimal(18,2)");

            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
