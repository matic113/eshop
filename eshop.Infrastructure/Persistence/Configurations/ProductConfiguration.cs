using eshop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eshop.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasIndex(p => p.ProductCode);

            builder.HasIndex(p => p.SellerId);

            builder.Property(p => p.ProductCode)
                .HasMaxLength(50);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .IsRequired();

            builder.Property(p => p.NameArabic)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.DescriptionArabic)
                .IsRequired();

            builder.Property(p => p.Price)
                .IsRequired()
                .HasDefaultValue(0)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Stock)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(p => p.CoverPictureUrl)
                .IsRequired();

            builder.Property(p => p.Weight)
                .HasColumnType("decimal(8,3)");

            builder.Property(p => p.Color)
                .HasMaxLength(20);

            builder.Property(p => p.DiscountPercentage)
                .HasDefaultValue(0)
                .HasColumnType("smallint");

            // Filter

            builder.HasIndex(p => p.IsDeleted);

            builder.HasQueryFilter(p => !p.IsDeleted);

            // Relationships
            builder
                .HasMany(p => p.Categories)
                .WithMany(c => c.Products);

            builder.HasMany(p => p.ProductPictures)
                .WithOne(pp => pp.Product)
                .HasForeignKey(pp => pp.ProductId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Reviews)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
