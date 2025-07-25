using eshop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eshop.Infrastructure.Persistence.Configurations
{
    public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
    {
        public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.OrderId);

            builder.Property(x => x.OrderStatus)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(x => x.Notes)
                .HasMaxLength(250);
        }
    }
}
