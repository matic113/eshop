using eshop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eshop.Infrastructure.Persistence.Configurations
{
    public class SellerConfiguration : IEntityTypeConfiguration<Seller>
    {
        public void Configure(EntityTypeBuilder<Seller> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasIndex(s => s.UserId);

            builder.Property(s => s.StoreName)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Seller>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
